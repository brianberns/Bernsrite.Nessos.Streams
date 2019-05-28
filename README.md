# Extensions for Nessos streams.

## Overview

This is a .NET class library that extends Nessos.Streams. The extensions fall into three categories:

1. Additional `Stream` functions that mirror the built-in F# `Seq` API. E.g. `Stream.append`,
   `Stream.average`, etc.
2. Additional `Stream` functions that complement Nessos functions that mix both `Seq` and `Stream`.
   E.g. `Stream.groupByStreams` returns a stream of streams, whereas the Nessos `Stream.groupBy`
   function returns a stream of sequences.
3. A builder object for stream comprehensions. E.g.
```F#
let data = [| 1..1000000 |]
stream {
    for x in data do
        if x % 2 = 0 then
            yield x * x
}
```
