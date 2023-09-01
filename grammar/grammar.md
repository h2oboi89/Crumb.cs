
# crumb.ebnf

## 


### Program

![Program](.\diagram/Program.svg)

References: [Start](#Start), [Block](#Block), [End](#End)

### Block

![Block](.\diagram/Block.svg)

Used by: [Program](#Program), [Block](#Block), [Define](#Define)
References: [Block](#Block), [Statement](#Statement)

### Statement

![Statement](.\diagram/Statement.svg)

Used by: [Block](#Block)
References: [Apply](#Apply), [Define](#Define)

### Apply

![Apply](.\diagram/Apply.svg)

Used by: [Statement](#Statement)
References: [Identifier](#Identifier), [Atom](#Atom)

### Define

![Define](.\diagram/Define.svg)

Used by: [Statement](#Statement)
References: [Identifier](#Identifier), [List](#List), [Block](#Block)

### Atom

![Atom](.\diagram/Atom.svg)

Used by: [Apply](#Apply), [List](#List)
References: [List](#List), [Integer](#Integer), [Float](#Float), [String](#String), [Identifier](#Identifier)

### List

![List](.\diagram/List.svg)

Used by: [Define](#Define), [Atom](#Atom)
References: [Atom](#Atom)

### Integer

![Integer](.\diagram/Integer.svg)

Used by: [Atom](#Atom)
References: [Digit](#Digit)

### Float

![Float](.\diagram/Float.svg)

Used by: [Atom](#Atom)
References: [Digit](#Digit)

### Digit

![Digit](.\diagram/Digit.svg)

Used by: [Integer](#Integer), [Float](#Float)

### String

![String](.\diagram/String.svg)

Used by: [Atom](#Atom)

### Identifier

![Identifier](.\diagram/Identifier.svg)

Used by: [Apply](#Apply), [Define](#Define), [Atom](#Atom)

