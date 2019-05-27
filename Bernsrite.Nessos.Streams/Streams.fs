namespace Bernsrite.Nessos.Streams

open Nessos.Streams

module Stream =

    let where = Stream.filter

    /// <summary>Applies a key-generating function to each element of the input stream and yields a stream of unique keys and a stream of all elements that have each key.</summary>
    /// <param name="projection">A function to transform items of the input stream into comparable keys.</param>
    /// <param name="source">The input stream.</param>
    /// <returns>A stream of tuples where each tuple contains the unique key and a stream of all the elements that match the key.</returns>    
    let groupByStreams (projection : 'elem -> 'key) source =
        source
            |> Stream.groupBy projection
            |> Stream.map (fun (key, grouping) ->
                key, Stream.ofSeq grouping)

    /// <summary>Concatenates a stream of streams.</summary>
    /// <param name="streams">The stream of streams to concatenate.</param>
    /// <returns>The concatenated stream.</returns>
    let concatStreams streams =
        streams
            |> Stream.toSeq
            |> Stream.concat
    
type StreamBuilder() =

    member __.Combine(streamA, streamB) =
        Stream.concat [streamA; streamB]

    member __.Delay(f) =
        f()

    member __.Yield(item) =
        Stream.ofSeq [item]

    member __.YieldFrom(items) =
        Stream.ofSeq items

[<AutoOpen>]
module Builder =
    let stream = new StreamBuilder()
