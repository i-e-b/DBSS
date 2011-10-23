using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace DBSS.Calculations {

	public enum TokenClass {
		Operand,
		UniaryPostfix, UniaryPrefix,
		ArgumentSeperator,
		BinaryOperator,
		OpenBracket, CloseBracket,
		Function,
		Name // For variables and anything unknown.
	}
	public enum Association {
		LeftToRight,
		RightToLeft
	}

	/// <summary>
	/// Holds token information
	/// </summary>
	class Token {
		public TokenClass Class { get; set; }
		public int Precedence { get; set; }
		public string Value { get; set; }
		public Association Direction { get; set; }

		public bool IsEmpty { get { return String.IsNullOrEmpty(Value); } }

		public Token (string value) {
			Value = value;
			Class = value.Class();
			Precedence = value.Precedence();
			Direction = value.Associativity();
		}

		/// <summary>
		/// Compare precedence, taking associativity into account
		/// </summary>
		public bool ShouldDisplace (Token other) {
			if (other.Class != TokenClass.BinaryOperator) return false;
			if (Direction == Association.LeftToRight) {
				return other.Precedence >= Precedence;
			}
			return other.Precedence > Precedence;
		}

		public override string ToString () {
			return Value;
		}

		public static implicit operator string (Token t) {
			return t.Value;
		}
	}

	/// <summary>
	/// Tokeniser stuff moved out to keep code clean
	/// </summary>
	static class Tokeniser {
		/// <summary>
		/// Split an expression into tokens
		/// </summary>
		public static List<Token> Tokens (this string input) {
			// (strings) | (nums, funcs, vars) | (sci nums)
			var r = new Regex(@"(\['.*?'\])|([^0-9a-zA-Z.\$])|([0-9.]+e[+\-]?[0-9]+)");
			// anything but alpha numeric or decimals gets split
			var output = new List<Token>();
			// return splits, including split characters (because of capture group in regex)

			string[] frags = r.Split(input);

			foreach (string frag in frags) {
				if (String.IsNullOrEmpty(frag)) continue;

				var t = new Token(frag);

				var last = (output.Count > 0) ? (output.Last()) : (null);

				#region check for uniary prefix operators
				if (t.Class == TokenClass.BinaryOperator) {
					if (output.Count < 1) {
						t.Class = TokenClass.UniaryPrefix;
					} else {
						if (last != null)
							switch (last.Class) {
								case TokenClass.BinaryOperator:
								case TokenClass.UniaryPostfix:
								case TokenClass.OpenBracket:
								case TokenClass.ArgumentSeperator:
									t.Class = TokenClass.UniaryPrefix;
									break;
							}
					}
				}
				#endregion

				#region check for functions
				// a name followed by an open bracket is taken to be a function
				if (last != null)
					if (t.Class == TokenClass.OpenBracket
					    && output.Count > 0
					    && last.Class == TokenClass.Name) {

						last.Class = TokenClass.Function;
						last.Value = "/" + last; // easier to find functions, plus vars and fucntions can share a name
					}
				#endregion

				output.Add(t);
			}
			return output;
		}

		/// <summary>
		/// Determine the class of a token
		/// </summary>
		public static TokenClass Class (this string input) {
			switch (input.ToLowerInvariant()) {
				case "+": return TokenClass.BinaryOperator;
				case "-": return TokenClass.BinaryOperator;
				case "*": return TokenClass.BinaryOperator;
				case "/": return TokenClass.BinaryOperator;
				case "^": return TokenClass.BinaryOperator;
				case "%": return TokenClass.BinaryOperator;
				case "!": return TokenClass.UniaryPostfix;
				case "(": return TokenClass.OpenBracket;
				case ")": return TokenClass.CloseBracket;
				case ",": return TokenClass.ArgumentSeperator;
				default: // not a simple token. Check for number or other
					double dummy;
					if (double.TryParse(input, out dummy)) return TokenClass.Operand; // inefficient, but gets the job done
					return TokenClass.Name; // no idea what this token is -- should be a var or func name.
			}
		}

		/// <summary>
		/// Determine the precedence of a token.
		/// Higher number are higher precedence
		/// </summary>
		public static int Precedence (this string input) {
			switch (input.ToLowerInvariant()) {
				case ",": return 1;
				case "+": return 2;
				case "-": return 2;
				case "*": return 3;
				case "/": return 3;
				case "%": return 3;
				case "^": return 4;
				case "!": return 5;
				case "(": return 6;
				case ")": return 6;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Determine the association direction of a token
		/// </summary>
		public static Association Associativity (this string input) {
			switch (input.ToLowerInvariant()) {
					// L-R
				case ",": return Association.LeftToRight;
				case "+": return Association.LeftToRight;
				case "-": return Association.LeftToRight;
				case "*": return Association.LeftToRight;
				case "/": return Association.LeftToRight;
				case "%": return Association.LeftToRight;
				case "!": return Association.LeftToRight;
				case "(": return Association.LeftToRight;
				case ")": return Association.LeftToRight;
					// R-L
				case "^": return Association.RightToLeft;
				default:
					return Association.LeftToRight;
			}
		}

		public static bool HasItems (this Stack<Token> input) {
			return (input.Count > 0 && input.Peek() != null);
		}
	}

	public class ResultSet {
		public string Result { get; set; }
		public List<CellReference> References { get; set; }
		public ResultSet () {
			References = new List<CellReference>();
		}
	}

	public class Calculator {
		private readonly Functions funcs;

		public Calculator (SparseArray array) {
			funcs = new Functions(array);
		}

		/// <summary>
		/// Transform an infix expression string into
		/// a postfix expression string
		/// </summary>
		public Stack<string> InfixToPostfix (string expression) {
			var operands = new Stack<Token>();
			var postfix = new Stack<Token>();

			foreach (Token token in expression.Tokens()) {
				if (token.IsEmpty) continue;

				switch (token.Class) {
					case TokenClass.Name:
					case TokenClass.Operand:
						#region Test for uniary operator
						if (operands.HasItems()
							&& operands.Peek().Class == TokenClass.UniaryPrefix) {
							switch (operands.Peek().Value) {
								case "-":
									// turn a uniary minus and operand into a negative operand
									operands.Pop();
									postfix.Push(token);
									postfix.Push(new Token("-1"));
									postfix.Push(new Token("*"));
									break;
								case "+":
									// no change to operand, remove uniary
									postfix.Push(token);
									operands.Pop();
									break;
								default:
									throw new Exception("Unexpected operator");
							}

						#endregion
						} else {
							postfix.Push(token);
						}
						break;
					case TokenClass.UniaryPostfix:
						operands.Push(token);
						break;
					case TokenClass.UniaryPrefix:
					case TokenClass.Function:
					case TokenClass.OpenBracket:
						#region Test for uniary operator
						if (operands.HasItems()
							&& operands.Peek().Class == TokenClass.UniaryPrefix) {
							switch (operands.Peek().Value) {
								case "-":
									// change value, which will get picked up on bracket close
									token.Value = "-";
									operands.Pop(); // remove uniary
									operands.Push(token); // push bracket
									break;
								case "+":
									operands.Pop(); // remove uniary
									operands.Push(token); // push bracket
									break;
								default:
									throw new Exception("Unexpected operator");
							}
						#endregion
						} else {
							operands.Push(token);
						}
						break;

					case TokenClass.ArgumentSeperator:
						while (operands.HasItems()
							&& operands.Peek().Class != TokenClass.OpenBracket) {
							if (operands.Count < 1) throw new Exception("Argument seperator outside of argument list");
							postfix.Push(operands.Pop());
						}
						break;

					case TokenClass.BinaryOperator:
						while (operands.HasItems()
							&& token.ShouldDisplace(operands.Peek())) { // compare precedence
							postfix.Push(operands.Pop());
						}
						operands.Push(token);
						break;

					case TokenClass.CloseBracket:
						while (operands.HasItems()
							&& operands.Peek().Class != TokenClass.OpenBracket) { // add inner bracket contents
							postfix.Push(operands.Pop());
						}
						#region Check for previously caught uniary minus bracket "-(...)"
						if (operands.HasItems()) {
							Token ob = operands.Pop(); // check open bracket
							if (ob.Value == "-") { // invert value
								postfix.Push(new Token("-1"));
								postfix.Push(new Token("*"));
							}
						}
						#endregion
						if (operands.HasItems()
							&& operands.Peek().Class == TokenClass.Function) {// this is actually a function
							postfix.Push(operands.Pop());
						}
						break;

					default:
						throw new Exception("Unexpected token: " + token.Value);
				} // end of switch
			} // end of token stream

			// deal with any remaining operators
			while (operands.HasItems()) { // compare precedence
				postfix.Push(operands.Pop());
			}

			var output = new Stack<string>(postfix.Count);
			foreach (Token t in postfix) output.Push(t.Value.Trim());
			return output;
		}

		/// <summary>
		/// Evaluate the result of a postfix expression string.
		/// Expected to be space-delimited
		/// </summary>
		public ResultSet EvaluatePostfix (Stack<string> postfix, int row, int column) {
			// very simple for the moment, doesn't handle variables or functions

			var @out = new ResultSet();

			var values = new Stack<CValue>();
			values.Push(new CValue((decimal)0.0)); // quick hack to deal with leading uniary operators

			foreach (string token in postfix/*.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)*/) {
				if (String.IsNullOrEmpty(token)) continue;
				decimal value;
				if (decimal.TryParse(token, out value)) {
					values.Push(new CValue(value));
					continue;
				}
				CValue l, r;

				switch (token.ToLowerInvariant()) {
					case "+":
						r = values.Pop(); l = values.Pop();
						values.Push(l + r);
						break;
					case "-":
						r = values.Pop(); l = values.Pop();
						values.Push(l - r);
						break;
					case "*":
						r = values.Pop(); l = values.Pop();
						values.Push(l * r);
						break;
					case "/":
						r = values.Pop(); l = values.Pop();
						values.Push(l / r);
						break;
					case "^":
						r = values.Pop(); l = values.Pop();
						values.Push(CValue.Pow(l, r));
						break;
					case "%":
						r = values.Pop(); l = values.Pop();
						values.Push(l % r);
						break;
					case "!":
						l = CValue.Floor(values.Pop());
						decimal f = 1;
						for (var c = (int)l.NumericValue; c > 0; c--) { f *= c; }
						values.Push(new CValue(f));
						break;

					// not used in postfix notation:
					case ",":
					case "(":
					case ")":
						throw new Exception("Unexpected token in Postfix");

					default:
						if (token.StartsWith("['") && token.EndsWith("']")) {
							// it's a string literal. Like this: ['String literal']
							values.Push(new CValue(token.Substring(2, token.Length - 4)));
						} else {
							@out.References.AddRange(
								funcs.HandleFunction(token, values, column, row)
								);
						}
						break;
				}

			} // end foreach

			@out.Result = values.Pop().ToString();

			return @out;
		}

	}
}
