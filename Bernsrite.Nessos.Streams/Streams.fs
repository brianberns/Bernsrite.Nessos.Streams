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

    let contains value stream =
        stream
            |> Stream.tryFind ((=) value)
            |> Option.isSome

    let distinct stream =
        stream
            |> Stream.toSeq
            |> Seq.distinct
            |> Stream.ofSeq

    let exactlyOne stream =
        stream
            |> Stream.toSeq
            |> Seq.exactlyOne

    let init count initializer =
        Seq.init count initializer
            |> Stream.ofSeq

    let iteri action stream =
        stream
            |> Stream.mapi (fun i elem -> i, elem)
            |> Stream.iter (fun (i, elem) -> action i elem)

    let max stream =
        stream
            |> Stream.maxBy id

    let min stream =
        stream
            |> Stream.minBy id

    let pairwise stream =
        stream
            |> Stream.toSeq
            |> Seq.pairwise
            |> Stream.ofSeq

    let skipWhile predicate stream =
        stream
            |> Stream.toSeq
            |> Seq.skipWhile predicate
            |> Stream.ofSeq

    let sort stream =
        stream
            |> Stream.sortBy id

    let sumBy projection stream =
        stream
            |> Stream.minBy projection
            |> Stream.sum

    let tail stream =
        stream
            |> Stream.skip 1

    let truncate = Stream.take   // current implementation of `take` actually implements `truncate` functionality

    /// Filters the elements of the input stream.
    let where = Stream.filter

    let zip stream1 stream2 =
        Stream.zipWith
            (fun elem1 elem2 -> elem1, elem2)
            stream1
            stream2
    
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
