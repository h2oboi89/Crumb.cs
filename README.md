# crumb.cs
Inspired by https://github.com/liam-ilan/crumb. Written in C# because I love the language and wanted to play around with .NET 7.

Deviated from the original enough that it should probably get a different name but I really like the ability to say "I'm crumbing!"

## Grammar

```ebnf
Program     ::= Start Scope End;

Scope       ::= '{' ( Statement )* '}';

Statement   ::= '(' Identifier ( Atom ) ')';

Atom        ::= List | Integer | Float| String | Identifier | Scope;

List        ::= '(' ( Atom )* ')';

Integer     ::= [ '-' ] ( Digit )+;

Float       ::= [ '-' ] ( Digit )+ '.' ( Digit )+;

String      ::= '"' .* '"';

Identifier  ::= .*;

Digit       ::= ( '0-9' )+;
```

[Grammar](./grammar/index.md)