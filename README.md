# A7 programming language

> **Warning** Still in development

Inspired by Jai, Odin, Zig and C Programming languages

## Build/Run
requires .net7.0+

- `dotnet build`
- `dotnet run`


## Example 
```cpp
io :: import "std/io"

add :: fn(x: i32, y: i32) i32 { ret x + y }

main :: fn() {
    io.print("Hello, World %\n", add(1, 2))
}
```