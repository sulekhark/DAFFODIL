// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
class T8
{
    static void Main(string[] args)
    {
        Console.Write("Please enter two numbers: ");

        try
        {
            int num1 = int.Parse(Console.ReadLine());
            int num2 = int.Parse(Console.ReadLine());

            int result = num1 / num2;

            Console.WriteLine("{0} / {1} = {2}", num1, num2, result);
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine("Cannot divide by zero. Please try again.{0}", ex.Message);
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Trying something else {0}", ex.Message);
            try
            {
                foo();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Not a valid number. Please try again.{0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("finally block");
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine("last catch {0}", e.Message);
        }
        Console.ReadKey();
    }

    static void foo()
    {
        throw new Exception();
    }

}