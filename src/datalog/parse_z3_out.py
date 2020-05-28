#!/usr/bin/env python3

# Author: Sulekha Kulkarni
# Date: January 2020

import logging
import re
import sys

# Note: Parsing is sensitive to white space. Does not handle it elegantly.

analysisFileName = sys.argv[1]
outputFileName = sys.argv[2]

logging.basicConfig(level=logging.INFO, \
                    format="[%(asctime)s] %(levelname)s [%(name)s.%(funcName)s:%(lineno)d] %(message)s", \
                    datefmt="%H:%M:%S")

relNameToDomFN = {}

####################################################################
# First get relation signatures - populates relNameToDomFN

def getRelationDefn(line):
    relDefn = ""
    if ("printtuples" in line):
        relDefn = line.split(' ', 1)[0]
    return relDefn


def insertDict(relDefn):
    partsArr = relDefn.split(',')
    domFN = []
    for partStr in partsArr:
        domName = partStr.split(':',1)[1]
        domName = domName.split(')',1)[0]
        domFileName = domName + ".map"
        domFN.append(domFileName)
    relName = relDefn.split('(',1)[0]
    relNameToDomFN[relName] = domFN


for line in open(analysisFileName):
    line = line.strip()
    relDefn = getRelationDefn(line)
    if relDefn:
        insertDict(relDefn)
print(relNameToDomFN)


####################################################################    
# Parse Z3 output to produce .datalog and .txt files for the output relations

currRelName = None
currRelDlogEntries = []
currRelTxtEntries = []
currRelDomElems = []

def relationStart(line):
    global currRelName
    if (currRelName):
        saveRelation()
    partsArr = line.split(' ')
    currRelName = partsArr[2]
    currRelName = currRelName[:-1] 
    currRelDomFN = relNameToDomFN[currRelName]
    for dfn in currRelDomFN:
        domList = [line.strip() for line in open(dfn)]
        currRelDomElems.append(domList)


def saveRelation():
    global currRelName
    global currRelDlogEntries
    global currRelTxtEntries
    currRelDlogFN = currRelName + ".datalog"
    currRelDlogFile = open(currRelDlogFN, 'w')
    for entry in currRelDlogEntries:
        print(entry, file=currRelDlogFile)
    currRelDlogFile.close()

    currRelTxtFN = currRelName + ".txt"
    currRelTxtFile = open(currRelTxtFN, 'w')
    for entry in currRelTxtEntries:
        print(entry, file=currRelTxtFile)
    currRelTxtFile.close()
    print("Saved " + currRelName)

    currRelName = ""
    currRelDlogEntries.clear()
    currRelTxtEntries.clear()
    currRelDomElems.clear()


def parseRelEntry(line):
    partsArr = line.split(',')
    dlogEntry = currRelName + "("
    txtEntry = currRelName + "("
    domCnt = 0
    for part in partsArr:
        idxStr = part.split('=',1)[1]
        idxStr = idxStr.split("(",1)[0]
        dlogEntry = dlogEntry + idxStr + ","
        idx = int(idxStr)
        txtEntry = txtEntry + idxStr + ":" + currRelDomElems[domCnt][idx] + ","
        domCnt = domCnt + 1
    dlogEntry = dlogEntry[:-1] + ")."
    currRelDlogEntries.append(dlogEntry)
    txtEntry = txtEntry[:-1] + ")."
    currRelTxtEntries.append(txtEntry)


for line in open(outputFileName):
    line = line.strip()
    if (line.startswith("Tuples in")):
        relationStart(line)
    elif (line.startswith("(")):
        parseRelEntry(line)
    elif currRelName:
        saveRelation()
