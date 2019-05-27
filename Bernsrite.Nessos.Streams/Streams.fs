namespace Bernsrite.Nessos.Streams

open Nessos.Streams

module Stream =

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

    let append stream1 stream2 =
        Stream.concat [stream1; stream2]

    let inline average stream =
        stream
            |> Stream.toSeq
            |> Seq.average

    let inline averageBy projection stream =
        stream
            |> Stream.toSeq
            |> Seq.averageBy projection

    let collect mapping stream =
        stream
            |> Stream.map mapping
            |> concatStreams

    /// Filters the elements of the input stream.
    let where = Stream.filter
    
type StreamBuilder() =

    member __.Combine(stream1, stream2) =
        Stream.concat [stream1; stream2]

    member __.Delay(f) =
        f()

    member __.For(items, f ) =
        items
            |> Seq.map f
            |> Stream.concat

    member __.Yield(item) =
        Stream.ofSeq [item]

    member __.YieldFrom(items) =
        Stream.ofSeq items

    member __.Zero() =
        Stream.empty

[<AutoOpen>]
module Builder =
    let stream = new StreamBuilder()
