%namespace QuickCalc.Parser.Parser
%partial
%parsertype ParserParser
%visibility internal
%tokentype Token

%union
{
	// public System.Numerics.Complex n; 
	public double n;
	public string  s;
}

%start line

%token
	NUMBER, VARIABLE, CONST, IMAGENARY,
	OP_LSHIFT, OP_RSHIFT, OP_XOR, OP_OR, OP_AND, OP_NOT
	OP_PLUS, OP_MINUS, OP_MULT, OP_DIV_INT, OP_DIV, OP_MOD, OP_POW, OP_FACTORIAL, OP_PERMUTATION, OP_COMBINATION
	P_OPEN, P_CLOSE, OP_MATH_FUNC, OP_MATH_FUNC2, OP_SEPARATOR, OP_LET, ERROR

%%

line
	: func3                 { Answer=$1.n;                                          /* Console.WriteLine("yacc [line] result : {0}\n", $1.n);     */ }
	| VARIABLE OP_LET func3 { Answer=$3.n; SetVariable($1.s,$3.n);                  /* Console.WriteLine("yacc [line] let : {0}\n", $3.n);        */ }
	;
func3
	: exp                  { $$.n = $1.n;                                          /* Console.WriteLine("yacc [func3] func3 : {0}", $$.n);          */ }
	| func3 OP_SEPARATOR OP_MATH_FUNC  { $$.n = CalcMathFunction($3.s,(double)$1.n); /* Console.WriteLine("yacc [func3] func factor : {0}", $$.n); */ }
	;
exp
	: exp2                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp] exp2 : {0}", $$.n);          */ }
	| exp OP_OR exp2       { $$.n = (int)$1.n | (int)$3.n;                         /* Console.WriteLine("yacc [exp] exp << term : {0}", $$.n);   */ }
	;
exp2
	: exp3                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp2] exp3 : {0}", $$.n);         */ }
	| exp2 OP_XOR exp3     { $$.n = (int)$1.n ^ (int)$3.n;                         /* Console.WriteLine("yacc [exp2] exp2 << term : {0}", $$.n); */ }
	;
exp3
	: exp4                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp3] exp4 : {0}", $$.n);         */ }
	| exp3 OP_AND exp4     { $$.n = (int)$1.n & (int)$3.n;                         /* Console.WriteLine("yacc [exp3] exp3 << term : {0}", $$.n); */ }
	;
exp4
	: exp5                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp4] exp5 : {0}", $$.n);         */ }
	| exp4 OP_LSHIFT exp5  { $$.n = (int)$1.n << (int)$3.n;                        /* Console.WriteLine("yacc [exp4] exp4 << exp5 : {0}", $$.n); */ }
	| exp4 OP_RSHIFT exp5  { $$.n = (int)$1.n >> (int)$3.n;                        /* Console.WriteLine("yacc [exp4] exp4 >> exp5 : {0}", $$.n); */ }
	;
exp5
	: exp6                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp5] exp6 : {0}", $$.n);        */  }
	| exp5 OP_PLUS exp6    { $$.n = $1.n + $3.n;                                   /* Console.WriteLine("yacc [exp5] exp5 + exp6 : {0}", $$.n); */  }
	| exp5 OP_MINUS exp6   { $$.n = $1.n - $3.n;                                   /* Console.WriteLine("yacc [exp5] exp5 - exp6 : {0}", $$.n); */  }
	;
exp6
	: exp7                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp6] exp7 : {0}", $$.n);         */ }
	| exp6 OP_MULT exp7    { $$.n = $1.n * $3.n;                                   /* Console.WriteLine("yacc [exp6] exp6 * exp7 : {0}", $$.n);  */ }
	| exp6 OP_DIV_INT exp7 { $$.n = (int)($1.n / $3.n);                            /* Console.WriteLine("yacc [exp6] exp6 // exp7 : {0}", $$.n); */ }
	| exp6 OP_DIV  exp7    { $$.n = $1.n / $3.n;                                   /* Console.WriteLine("yacc [exp6] exp6 / exp7 : {0}", $$.n);  */ }
	| exp6 OP_MOD  exp7    { $$.n = $1.n % $3.n;                                   /* Console.WriteLine("yacc [exp6] exp6 % exp7 : {0}", $$.n);  */ }
	; 
exp7
	: func                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp7] func : {0}", $$.n);         */ }
	| exp7 OP_POW func     { $$.n = Math.Pow((double)$1.n,(double)$3.n);  /* Console.WriteLine("yacc [exp7] exp7 ^ func : {0}", $$.n);  */ }
	| exp7 OP_FACTORIAL    { $$.n = Factorial((double)$1.n);              /* Console.WriteLine("yacc [exp7] exp7 ! : {0}", $$.n);  */ }
	;
func
	: func2                { $$.n = $1.n;                                          /* Console.WriteLine("yacc [func] factor : {0}", $$.n);       */ }
	| OP_MATH_FUNC func2   { $$.n = CalcMathFunction($1.s,(double)$2.n);  /* Console.WriteLine("yacc [func] func factor : {0}", $$.n);  */ }
	| OP_MATH_FUNC         { $$.n = CalcMathFunction($1.s);               /* Console.WriteLine("yacc [func] func : {0}", $$.n);  */ }
	;
func2
	: exp8                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [func2] factor : {0}", $$.n);      */ }
	| func2 OP_MATH_FUNC2  { $$.n = CalcMathFunction2($2.s,(double)$1.n); /* Console.WriteLine("yacc [func2] func factor : {0}", $$.n); */ }
	;
exp8
	: exp9                 { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp8] func : {0}", $$.n);         */ }
	| OP_NOT exp9          { $$.n = ~(int)$2.n;                                    /* Console.WriteLine("yacc [exp8] ~func : {0}", $$.n);       */ }
	| OP_MINUS exp9        { $$.n = -$2.n;                                         /* Console.WriteLine("yacc [exp8] - func : {0}", $$.n);       */ }
	;
exp9
	: factor               { $$.n = $1.n;                                          /* Console.WriteLine("yacc [exp9] func : {0}", $$.n);         */ }
	| exp9 OP_PERMUTATION factor { $$.n = Permutation( (double)$1.n, (double)$3.n ); /* Console.WriteLine("yacc [exp9] nPr : {0}", $$.n);       */ }
	| exp9 OP_COMBINATION factor { $$.n = Combination( (double)$1.n, (double)$3.n ); /* Console.WriteLine("yacc [exp9] nCr : {0}", $$.n);       */ }
	;
factor
	: number               { $$.n = $1.n;                                          /* Console.WriteLine("yacc [factor] number : {0}", $$.n);     */ }
	| P_OPEN exp P_CLOSE   { $$.n = $2.n;                                          /* Console.WriteLine("yacc [factor] ( exp ) : {0}", $$.n);    */ }
	;
number: 
	| NUMBER               { $$.n = $1.n;                                          /* Console.WriteLine("yacc [number] number : {0}", $$.n);     */ }
	| VARIABLE             { $$.n = GetVariable($1.s);                             /* Console.WriteLine("yacc [number] Variable : {0}", $$.n);   */ }
	| CONST                { $$.n = GetConst($1.s);                       /* Console.WriteLine("yacc [number] Const : {0}", $$.n);      */ }
	| OP_PLUS              { throw new ExpressionException("Unexpected operator"); }
	| OP_MINUS             { throw new ExpressionException("Unexpected operator"); }
	| OP_MULT              { throw new ExpressionException("Unexpected operator"); }
	| OP_DIV               { throw new ExpressionException("Unexpected operator"); }
	| OP_POW               { throw new ExpressionException("Unexpected operator"); }
	| OP_MATH_FUNC         { throw new ExpressionException("Unexpected operator"); }
	| OP_LET               { throw new ExpressionException("Unexpected operator"); }
	| OP_LSHIFT            { throw new ExpressionException("Unexpected operator"); }
	| OP_RSHIFT            { throw new ExpressionException("Unexpected operator"); }
	| OP_XOR               { throw new ExpressionException("Unexpected operator"); }
	| OP_OR                { throw new ExpressionException("Unexpected operator"); }
	| OP_AND               { throw new ExpressionException("Unexpected operator"); }
	| OP_NOT               { throw new ExpressionException("Unexpected operator"); }
	| OP_FACTORIAL         { throw new ExpressionException("Unexpected operator"); }
	| OP_PERMUTATION       { throw new ExpressionException("Unexpected operator"); }
	| OP_COMBINATION       { throw new ExpressionException("Unexpected operator"); }
	| IMAGENARY            { throw new ExpressionException("Not supported"); }
	;

%%
