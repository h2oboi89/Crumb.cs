
# crumb.ebnf

## 


### Program

![Program](./diagram/Program.svg)

References: [Start](#Start), [Block](#Block), [End](#End)

### Block

![Block](./diagram/Block.svg)

Used by: [Program](#Program), [Atom](#Atom)
References: [Apply](#Apply), [Atom](#Atom)

### Apply

![Apply](./diagram/Apply.svg)

Used by: [Block](#Block), [Apply](#Apply)
References: [Atom](#Atom), [Apply](#Apply)

### Atom

![Atom](./diagram/Atom.svg)

Used by: [Block](#Block), [Apply](#Apply), [List](#List)
References: [List](#List), [Integer](#Integer), [Float](#Float), [String](#String), [Boolean](#Boolean), [Void](#Void), [Identifier](#Identifier), [Block](#Block)

### List

![List](./diagram/List.svg)

Used by: [Atom](#Atom)
References: [Atom](#Atom)

### Integer

![Integer](./diagram/Integer.svg)

Used by: [Atom](#Atom)
References: [Digit](#Digit)

### Float

![Float](./diagram/Float.svg)

Used by: [Atom](#Atom)
References: [Digit](#Digit)

### Digit

![Digit](./diagram/Digit.svg)

Used by: [Integer](#Integer), [Float](#Float)

### String

![String](./diagram/String.svg)

Used by: [Atom](#Atom)

### Boolean

![Boolean](./diagram/Boolean.svg)

Used by: [Atom](#Atom)

### Void

![Void](./diagram/Void.svg)

Used by: [Atom](#Atom)

### Identifier

![Identifier](./diagram/Identifier.svg)

Used by: [Atom](#Atom)

