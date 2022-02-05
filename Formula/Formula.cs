﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Author: Joe Zachary, Daniel Kopta, H. James de St. Germain & Abhiveer Sharma
    /// Partner: None
    /// Date of Creation: Feburary 4, 2022
    /// Course: CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Abhiveer Sharma - This work may not be copied for use in Academic Coursework. 
    /// I, Abhiveer Sharma, certify that I wrote this code from scratch and did not copy it in part or whole from  
    /// another source. All references used in the completion of the assignment are cited in my README file. 
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private HashSet<string> variables;
        private string _formula;
        private List<string> tokens;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {

            int counter = 0;
            string prevToken = "";
            variables = new HashSet<string>();
            _formula = "";
            tokens = new List<string>();
            foreach (string token in GetTokens(formula))
            {
                if (isNumber(token))
                {
                    string numToken = Double.Parse(token).ToString();
                    tokens.Add(numToken);
                    _formula += numToken;
                }
                if (isVariable(token))
                {
                    string normalizedToken = normalize(token);
                    if ((isVariable(normalizedToken) && isValid(normalizedToken)))
                    {
                        variables.Add(normalizedToken);
                        tokens.Add(normalizedToken);
                        _formula += normalizedToken;
                    }
                    else
                    {
                        throw new FormulaFormatException("Invalid token");
                    }

                }
                if (isOperator(token) || token.Equals("(") || token.Equals(")"))
                {
                    tokens.Add(token);
                    _formula += token;
                }
                //1. Specific Token Rule
                if (!(token.Equals("(") || token.Equals(")") || isOperator(token) || isNumber(token) || isVariable(token)))
                {

                    throw new FormulaFormatException("Specific Token rule violated !");

                }
                //3. Right Parentheses Rule
                if (token.Equals(")"))
                {
                    counter++;
                }

                if (token.Equals("("))
                {
                    counter--;
                }
                if (counter > 0)
                {
                    throw new FormulaFormatException("Right Parentheses rule violated !");
                }
                //7. Parenthesis/Operator Following Rule
                if (prevToken.Equals("(") || isOperator(prevToken))
                {
                    if (!(isNumber(token) || isVariable(token) || token.Equals("(")))
                    {
                        throw new FormulaFormatException("Parenthesis/Operator Following rule violated !");
                    }
                }
                //8. Extra Following Rule
                if (isNumber(prevToken) || isVariable(prevToken) || prevToken.Equals(")"))
                {
                    if (!(isOperator(token) || token.Equals(")")))
                    {
                        throw new FormulaFormatException("Extra Following rule violated !");
                    }
                }
                prevToken = token;

            }
            //4. Balanced Parantheses rule
            if (counter != 0)
            {
                throw new FormulaFormatException("Balanced Parantheses rule has been violated !");
            }
            //6. Ending Token Rule 
            if (!(isNumber(prevToken) || isVariable(prevToken) || prevToken.Equals(")")))
            {
                throw new FormulaFormatException("Ending Token rule violated !");
            }
            //5. Starting Token Rule
            if (!(isNumber(tokens[0]) || isVariable(tokens[0]) || tokens[0].Equals("(")))
            {
                throw new FormulaFormatException("Starting Token rule violated !");
            }
            //2. One Token Rule
            if(tokens.Count == 0)
            {
                throw new FormulaFormatException("One Token rule violated !");
            }


        }

        /// <summary>
        /// Helper method for checking if the token is a legal variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Boolean isVariable(String token)
        {

            return Regex.IsMatch(token, "^[a-zA-Z_][a-zA-Z0-9_]*$");
        }
        /// <summary>
        /// Helper method for checking if the token is a number
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Boolean isNumber(String token)
        {
            return (double.TryParse(token, out _));

        }
        /// <summary>
        /// Helper method for checking if the token is one of four operators - '+','-','*','/'
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Boolean isOperator(String token)
        {
            if (token.Equals("+"))
            {
                return true;
            }
            if (token.Equals("-"))
            {
                return true;
            }
            if (token.Equals("*"))
            {
                return true;
            }
            if (token.Equals("/"))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            foreach (string token in tokens)
            {
                //This removes the white space.
                String _token = token.Trim();
                double numericValue;
                bool isNumber = double.TryParse(_token, out numericValue);
                try
                {
                    if (isNumber)
                    {
                        if (StackExtension.IsOnTop(operators, "*") || StackExtension.IsOnTop(operators, "/"))
                        {

                            double operand = values.Pop();
                            String _operator = operators.Pop();
                            double result = Calculate(operand, _operator, numericValue);
                            values.Push(result);
                        }

                        else
                        {
                            values.Push(numericValue);
                        }
                    }

                    if (isVariable(_token))
                    {
                        
                         double variableValue = lookup(_token);
                        if (StackExtension.IsOnTop(operators, "*") || StackExtension.IsOnTop(operators, "/"))
                        {

                            double operand = values.Pop();
                            String _operator = operators.Pop();

                            double result = Calculate(operand, _operator, variableValue);
                            values.Push(result);
                        }

                        else
                        {
                            values.Push(variableValue);
                        }
                       
                    }

                    if (((_token.Equals("+") || _token.Equals("-"))))
                    {
                        if (StackExtension.IsOnTop(operators, "+") || StackExtension.IsOnTop(operators, "-"))
                        {
                            if (values.Count < 2)
                            {
                                throw new ArgumentException();
                            }
                            double operand1 = values.Pop();
                            double operand2 = values.Pop();
                            string _operator = operators.Pop();
                            double result = Calculate(operand1, _operator, operand2);
                            values.Push(result);

                        }
                        operators.Push(_token);
                    }

                    if (_token.Equals("*") || _token.Equals("/"))
                    {
                        operators.Push(_token);
                    }

                    if (_token.Equals("("))
                    {
                        operators.Push(_token);
                    }

                    if (_token.Equals(")"))
                    {
                        if (values.Count >= 2 && (StackExtension.IsOnTop(operators, "+") || StackExtension.IsOnTop(operators, "-")))
                        {
                            double operand1 = values.Pop();
                            double operand2 = values.Pop();
                            string _operator = operators.Pop();
                            double result = Calculate(operand1, _operator, operand2);
                            values.Push(result);
                        }

                        if (StackExtension.IsOnTop(operators, "("))
                        {
                            operators.Pop();
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        if (values.Count >= 2 && (StackExtension.IsOnTop(operators, "*") || StackExtension.IsOnTop(operators, "/")))
                        {
                            double operand1 = values.Pop();
                            double operand2 = values.Pop();
                            string _operator = operators.Pop();
                            double result = Calculate(operand2, _operator, operand1);
                            values.Push(result);
                        }
                    }



                }
                catch (ArgumentException)
                {
                    return new FormulaError("Lookup couldn't find the value of variable");
                }
                catch (DivideByZeroException)
                {
                    return new FormulaError("Division by zero error !");
                }
            }

            // After the last token has been processed
            if (operators.Count == 0 && values.Count == 1)
            {
                return values.Pop();
            }

            else
            {
                
                    double operand1 = values.Pop();
                    double operand2 = values.Pop();
                    string _operator = operators.Pop();
                    double result = Calculate(operand1, _operator, operand2);
                    return result;
              
            }


        }
        /// <summary>
        /// This is a helper method made for calculating the value of arithmetic expression
        /// involving integers and one of the following operators - '+', '-', '*' & '/'.
        /// Throws an exception if division by zero occurs
        /// <param name="value1"></param> The first value popped off the value stack
        /// <param name="exp"></param> The operator popped off the operator stack
        /// <param name="value2"></param> The second value popped off the value stack
        /// <returns></returns> The final result after performing the mathematical operation
        /// <exception cref="ArgumentException"></exception>
        /// </summary>
        private static double Calculate(double value1, String exp, double value2)
        {
            if (exp.Equals("*"))
            {
                return value1 * value2;
            }
            if (exp.Equals("+"))
            {
                return value1 + value2;
            }
            if (exp.Equals("-"))
            {
                return value2 - value1;
            }

            if (exp.Equals("/"))
            {
                if (value2 == 0)
                {
                    throw new DivideByZeroException();
                }
                else
                {
                    return value1 / value2;
                }
            }

            return 0;

        }








        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            //use a hashset instance variables and add variables there in constructor
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            //use a string - _formula as instance variable and add tokens to it in constructor
            return _formula;
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (!(obj is Formula))
                return false;
            return obj.ToString() == _formula;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
           
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1==f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return _formula.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }
}
    

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }




// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
 /// <summary>
    /// This is an extension class written to help in the Evaluate method above in the Evaluator class
    /// This class implements the IsOnTop method
    /// The implementation of this class is inspired by Prof. Daniel Kopta's work.
    /// </summary>
    public static class StackExtension
    {
    /// <summary>
    /// This method checks the count of a stack (In our case Operator stack) and also finds out
    /// if the top of the stack matches object x
    /// <typeparam name="T"></typeparam> The type of the stack (In our case string)
    /// <param name="stack"></param> The stack whose count wi
    /// <param name="x"></param> The item at the top of the stack (In our case one of: +,-,/,*,(,)
    /// <returns></returns>
    /// </summary>
    public static Boolean IsOnTop<T>(this Stack<T> stack, T x) where T : notnull
    {
        if (stack.Count >= 1 && stack.Peek().Equals(x))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
    }


