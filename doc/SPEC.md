# Language spec 
i decided to make the language super simple, but functional enough to use for simple personal projects

which will compile to a subset of C and then use a C compiler to generate executables


## Features
- Simple procedural language
- Basic inferreable type system
- C like language inspiration with modifications for modernization
- prefer minimal typing
- Semicolons are optional


## Maybe? features
- Function overloading
 
- Basic generics sets (which are predefined generic types for a set of types), for example uint for all unsigned ints sizes

- Default function arguments values
- Struct of Arrays stuff (performance stuff)

    
    for example
    N for numbers(ints+floats)

### things to discuss 
- Maybe features above
- elif syntax
- number types syntax 
 either uint, int etc. 
 or u32, i32, f32 etc. 
- loops syntax


## WILL NOT HAVE
- Exceptions
- Classes (Inheritence/Encapsulation etc. OOP stuff)
- Operator overloading 
- Full Polymorphic parameters support (aka generics) 
- Extra Syntax bloat that might blow up the complier src code
- Insert your X feature (prob NOT)


## Keywords
- del (delete memory(free))
- new (memory allocation)
- ints (i64, i32, etc.)
- unsigned ints (u64, u32 ...)
- floats (f32, f64 etc.)
- char (actually u8)
- true
- false
- bool (builtin type)
- fn (function)
- ret (return)
- import
- if 
- elif (is it needed?)
- else 
- for (for/foreach/while all in one)
- and (logical and)
- or (logical or)
- pub (public)
- match (switch but(default breaks every stmt + more stuff))
- enum (ints only, defaults to the smallest size)
- break (only in loops)
- fall (only in switch stmts(match))
- record (struct) (classes without methods)
- variant (tagged union)
- ref (pointer) (raw pointers, but pointer arithmatic is not allowed)
- nil (null)
- continue
- cast (change type (not all casts are allowed))
- defer
- as
- using 

### Tokens
see file in `src/compiler/fe/Token.cs` 

## Syntax examples

### Hello World Function

```cpp
io :: import "std/io"

main :: fn() {
    io.println("Hello World")
}
```

### comments 
```rs
// single line

/*
    multi line
    /* can be nested too */
*/
```


### function
```cpp
io :: import "std/io"

main :: fn() {
    io.print_i(add(1, 2))
    io.println("")
}
// to show the order of def is not important
add :: fn(x: int, y: int) int { ret x + y }
```

### If stmts

```cpp
if cond {

} else if cond2 {

} else {

}
```



### loops 
```d
for {
    // infinite loop
    break 
} 

cond := true
while cond { 
    break
}

for (i := 0; i < 10; i += 1) {
    // i++ or ++i is not allowed 
    // due to complexity of the design
}

foreach (i, v: arr) {
    
}
```

### Structs
```go
Vec2 :: record {
    x: uint;
    y: uint;
}
```

### Heap memory 
```cpp
x := new Vec2
@assert(x != nil, "x is nil") // todo: make handling errors better
x.x = 1; x.y = 2
del x

y := new [10]Vec2 // []Vec2 with size 10;
y.count // count is more accurate term than length 
del y //  
```


### enums 
```cpp
// strongly typed (not an integer)
// it will compile to integers under the hood tho
Vehicle :: enum {
    Bike, 
    Car
}  
```


### defer
```go
io :: import "std/io"

// an example
// the file read function is Stdlib thing
// and it might change
f := io.fopen("path.ext", .READ)
defer io.fclose(f)
// defer means run at the end of the scope
// but not like go's defer where is uses the function stack
// its more like run stmt before function exits (bfr returns or end of function)
```


### pub 

```rs
// allowed to be called from outsize of this file
// by default, functions are encapsulated by the file
pub incr :: fn(x: uint) {
    ret x + 1
}

```


### cast 

```d
cast(int, 1.0) // more readable
```


### match (switch) 

```cpp
Status :: enum {
    Success, Failure, Done
}

x := Status.Success;
match (x) {
    case Status.Success: { 
            fall // fallthrough 
        }
    case Status.Done:  {
        /* both succ and done reach here*/
        }
    else: {/* Status.Failure */}
}
```