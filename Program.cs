using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    private static void Main()
    {
        while (true)
        {
            Console.WriteLine("enter 'normal' for regular calculations or 'derivatives' for finding derivatives. type 'exit' to quit");
            var mode = Console.ReadLine().Trim().ToLower();

            if (mode == "exit")
                break;

            switch (mode)
            {
                case "normal":
                    Console.WriteLine("enter your equation:");
                    var equation = Console.ReadLine();
                    
                    if (equation != null)
                    {
                        var allTokens = Tokenize(equation);
                        var rpnInput = ShuntingYard(allTokens);
                        var result = Calculate(rpnInput);
                        Console.WriteLine("result: " + result);
                    }
                    break;
                
                case "d":
                    var toDifferentiate = Console.ReadLine();
                    
                    if (toDifferentiate != null)
                    {
                        var dTokens = Tokenize(toDifferentiate);
                        var dResult = Differentiate(dTokens);
                        Console.WriteLine("derivative: " + string.Join("", dResult));
                    }
                    break;
                
                default:
                    Console.WriteLine("invalid mode selected. choose 'normal' or 'derivatives'");
                    break;
            }
        }
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var token = new StringBuilder();
        foreach (var ch in input)
        {
            if (char.IsDigit(ch) || ch == '.' || ch == 'x' || ch == '^')
            {
                token.Append(ch);
            }
            else if (ch.IsOperator())
            {
                if (token.Length > 0)
                {
                    tokens.Add(token.ToString());
                    token.Clear();
                }
                tokens.Add(ch.ToString());
            }
        }

        if (token.Length > 0)
        {
            tokens.Add(token.ToString());
        }

        return tokens;
    }

    static List<string> ShuntingYard(List<string> tokens)
    {
        var operators = new Stack<string>();
        var outputQueue = new Queue<string>();

        foreach (var token in tokens)
        {
            if (token.IsNumber())
            {
                outputQueue.Enqueue(token);
            }
            else if (token.IsOperator())
            {
                while (operators.Count > 0 && Helpers.Priority[operators.Peek()[0]] >= Helpers.Priority[token[0]])
                {
                    outputQueue.Enqueue(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            outputQueue.Enqueue(operators.Pop());
        }

        return new List<string>(outputQueue);
    }


    static List<string> Differentiate(List<string> tokens)
    {
        var newTokens = new List<string>();
        var newToken = "";
        var stack = new Stack<string>();
        var c = "";

        foreach (var token in tokens)
        {
            if (token.IsNumber()) 
            {
                stack.Push(token);
            } 
            
            else if (token.Contains("x^"))
            {
                var splitted = token.Split("^");
                var power = int.Parse(splitted[1]);
                var newPower = power - 1;

                if (stack.Count > 0)
                {
                    c = stack.Pop();
                }
                
                if (newPower > 1)
                {
                    newToken = $"{c}{power}*x^{newPower}";
                }
                else
                {
                    newToken = $"{c}{power}*x";
                }

                newTokens.Add(newToken);
            }
            else
            {
                newTokens.Add(stack.Pop());
            }

            if (token.IsOperator())
            {
                newTokens.Add(token);
            }
        }

        return newTokens;
    }


    static double Calculate(List<string> rpnTokens)
    {
        var stack = new Stack<double>();

        foreach (var token in rpnTokens)
        {
            if (token.IsNumber())
            {
                stack.Push(double.Parse(token));
            }
            else if (token.IsOperator())
            {
                var num2 = stack.Pop();
                var num1 = stack.Pop();

                var localResult = token switch
                {
                    "+" => num1 + num2,
                    "-" => num1 - num2,
                    "*" => num1 * num2,
                    "/" => num1 / num2,
                    _ => throw new ArgumentOutOfRangeException()
                };
                stack.Push(localResult);
            }
        }

        return stack.Pop();
    }
}

internal static class Helpers
{
    public static readonly Dictionary<char, int> Priority = new()
    {
        { '+', 1 },
        { '-', 1 },
        { '*', 2 },
        { '/', 2 },
    };

    public static bool IsOperator(this char @char) => Priority.ContainsKey(@char);
    public static bool IsOperator(this string token) => token.Length == 1 && Priority.ContainsKey(token[0]);
    public static bool IsNumber(this string input) => double.TryParse(input, out _);
}
