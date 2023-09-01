# crumb.cs
Inspired by https://github.com/liam-ilan/crumb. Written in C# because I love the language and wanted to play around with .NET 7.

Deviated from the original enough that it should probably get a different name but I really like the ability to say "I'm crumbing!"

## Grammar

```ebnf
Program     ::= Start Block End;

Block       ::= '{' ( Block | Statement )* '}';

Statement   ::= ( Apply | Define )+;

Apply       ::= '(' Identifier ( Atom )* ')';

Define      ::= '(' 'define' Identifier List? Block ')';

Atom        ::= List | Integer | Float| String | Identifier;

List        ::= '[' ( Atom )* ']';

Integer     ::= [ '-' ] ( Digit )+;

Float       ::= [ '-' ] ( Digit )+ '.' ( Digit )+;

Digit       ::= '0-9'+;

String      ::= '"' [.]* '"';

Identifier  ::= [.]+;
```

[Grammar](./grammar/grammar.md)