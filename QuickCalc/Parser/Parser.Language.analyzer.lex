%namespace QuickCalc.Parser.Parser
%scannertype ParserScanner
%visibility internal
%tokentype Token

%option stack, minimize, parser, verbose, persistbuffer, noembedbuffers 

Eol             (\r\n?|\n)
NotWh           [^ \t\r\n]
Space           [ \t]
Number          [0-9,]+(\.[0-9]+)?(([Ee][\+\-][0-9]+)|([hkKMGTPEZYRQdcmunpfazyrq]|da)|([KMGTPEZY]i))?
HexNumber       0[xX][0-9A-Fa-f_]+
BinNumber       0[bB][01_]+
Imaginary       ([ij][0-9]+)|([0-9]+[ij])
Const           ([Pp][Ii])|[Aa]nswer\ to\ [Ll]ife\ the\ [Uu]niverse\ and\ [Ee]verything
// Const           ([Pp][Ii])|([Aa]nswer)\ to\ (the\ [Uu]ltimate\ [Qq]uestion\ of\ )?[Ll]ife[\,\ ]the\ [Uu]niverse[\,\ ]and\ [Ee]verything
// Const           ([Pp][Ii])
FnSin           ([Ss][Ii][Nn])
FnCos           ([Cc][Oo][Ss])
FnTan           ([Tt][Aa][Nn])
FnSqrt          ([Ss][Qq][Rr][Tt])
FnExp           ([Ee][Xx][Pp])
FnLn            ([Ll][Nn])
FnLog           ([Ll][Oo][Gg])
FnAbs           ([Aa][Bb][Ss])
FnInv           ([Ii][Nn][Vv])
FnInt           ([Ii][Nn][Tt])
OpFunction      ({FnSin}|{FnCos}|{FnTan}|{FnSqrt}|{FnExp}|{FnLn}|{FnLog}|{FnAbs}|{FnInv}|{FnInt})
OpFunction2     \:{OpFunction}
OpSeparator     \;
OpLet           \=
OpLShift        <<
OpRShift        >>
OpPlus          \+
OpMinus         \-
OpMult          \*
OpDivInt        \/\/
OpDiv           \/
OpMod           %|([Mm][Oo][Dd])
OpPow           (\^|\*\*)
OpFactorial     \!
OpXor           `|([Xx][Oo][Rr])
OpOr            \||([Oo][Rr])
OpAnd           \&|([Aa][Nn][Dd])
OpNot           \~|([Nn][Oo][Tt])
OpPermutation   [Pp]
OpCombination   [Cc]
POpen           \(
PClose          \)
Variable        \#[0-9A-Za-z_]+
DateTime        @[0-9:-T](Y|M|D|h|m|s|ms|us)?
Comment         \[[^\]]*\]
Unexpected      .

%{

%}

%%

{Number}        { /* Console.WriteLine( "LEX: Number" );     */ GetNumber();    return (int)Token.NUMBER; }
// {Imaginary}     { /* Console.WriteLine( "LEX: ImgNumber" );  */ GetImgNumber(); return (int)Token.NUMBER; }
{HexNumber}     { /* Console.WriteLine( "LEX: HexNumber" );  */ GetHexNumber(); return (int)Token.NUMBER; }
{BinNumber}     { /* Console.WriteLine( "LEX: BinNumber" );  */ GetBinNumber(); return (int)Token.NUMBER; }
{Variable}      { /* Console.WriteLine( "LEX: Variable" );   */ SetName();      return (int)Token.VARIABLE; }
{Const}         { /* Console.WriteLine( "LEX: Const" );      */ SetName();      return (int)Token.CONST; }

{Space}+        /* skip */
{Comment}       /* skip */

{OpLet}         { /* Console.WriteLine( "LEX: OpLet" );      */            return (int)Token.OP_LET; }
{OpLShift}      { /* Console.WriteLine( "LEX: OpLShift" );   */            return (int)Token.OP_LSHIFT; }
{OpRShift}      { /* Console.WriteLine( "LEX: OpRShift" );   */            return (int)Token.OP_RSHIFT; }
{OpXor}         { /* Console.WriteLine( "LEX: OpXor" );      */            return (int)Token.OP_XOR; }
{OpOr}          { /* Console.WriteLine( "LEX: OpOr" );       */            return (int)Token.OP_OR; }
{OpAnd}         { /* Console.WriteLine( "LEX: OpAnd" );      */            return (int)Token.OP_AND; }
{OpPlus}        { /* Console.WriteLine( "LEX: OpPlus" );     */            return (int)Token.OP_PLUS; }
{OpMinus}       { /* Console.WriteLine( "LEX: OpMinus" );    */            return (int)Token.OP_MINUS; }
{OpPow}         { /* Console.WriteLine( "LEX: OpPow" );      */            return (int)Token.OP_POW; }
{OpMult}        { /* Console.WriteLine( "LEX: OpMult" );     */            return (int)Token.OP_MULT; }
{OpDivInt}      { /* Console.WriteLine( "LEX: OpDivInt" );   */            return (int)Token.OP_DIV_INT; }
{OpDiv}         { /* Console.WriteLine( "LEX: OpDiv" );      */            return (int)Token.OP_DIV; }
{OpMod}         { /* Console.WriteLine( "LEX: OpMod" );      */            return (int)Token.OP_MOD; }
{OpPermutation} { /* Console.WriteLine( "LEX: OpPermutation" ); */         return (int)Token.OP_PERMUTATION; }
{OpCombination} { /* Console.WriteLine( "LEX: OpCombination" ); */         return (int)Token.OP_COMBINATION; }
{OpNot}         { /* Console.WriteLine( "LEX: OpNot" );      */            return (int)Token.OP_NOT; }
{OpFactorial}   { /* Console.WriteLine( "LEX: OpFactorial" );*/            return (int)Token.OP_FACTORIAL; }
{OpFunction}    { /* Console.WriteLine( "LEX: OpFunction" ); */ SetName(); return (int)Token.OP_MATH_FUNC; }
{OpFunction2}   { /* Console.WriteLine( "LEX: OpFunction2"); */ SetName(); return (int)Token.OP_MATH_FUNC2; }
{OpSeparator}   { /* Console.WriteLine( "LEX: OpSeparator"); */            return (int)Token.OP_SEPARATOR; }
{POpen}         { /* Console.WriteLine( "LEX: POpen" );      */            return (int)Token.P_OPEN; }
{PClose}        { /* Console.WriteLine( "LEX: PClose" );     */            return (int)Token.P_CLOSE; }
{Unexpected}    { /* Console.WriteLine( "LEX: Unexpected" ); */ SetName(); return (int)Token.ERROR; }

%%