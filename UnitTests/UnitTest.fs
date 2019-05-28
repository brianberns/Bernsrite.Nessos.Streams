namespace Bernsrite.Nessos.Streams

open System.Diagnostics

open Microsoft.VisualStudio.TestTools.UnitTesting
open Nessos.Streams

[<TestClassAttribute>]
type UnitTest() =

    let time msg func =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let result = func ()
        stopwatch.Stop()
        printfn "%s: %A" msg <| stopwatch.Elapsed
        result

    let assertEqualStreams stream1 stream2 =
        let array1 = stream1 |> Stream.toArray
        let array2 = stream2 |> Stream.toArray
        for (a, b) in Array.zip array1 array2 do
            Assert.AreEqual(a, b)

    [<TestMethod>]
    member __.GroupByStreams() =
        let data = [| 1L..1000000L |]
        let projection x = x % 2L
        let seqValue =
            (fun () ->
                data
                    |> Stream.ofArray
                    |> Stream.groupBy projection
                    |> Stream.map (fun (x, grouping) ->
                        x, Seq.sum grouping))
                |> time "groupBy"
        let streamValue =
            (fun () ->
                data
                    |> Stream.ofArray
                    |> Stream.groupByStreams projection
                    |> Stream.map (fun (x, grouping) ->
                        x, Stream.sum grouping))
                |> time "groupByStreams"
        assertEqualStreams seqValue streamValue

    [<TestMethod>]
    member __.ConcatStreams() =
        let data =
            [| 1L..1000000L |]
                |> Array.groupBy (fun x -> x % 2L)
                |> Array.map snd
        let seqValue =
            (fun () ->
                data
                    |> Seq.map Stream.ofArray
                    |> Stream.concat)
                |> time "concat"
        let streamValue =
            (fun () ->
                data
                    |> Seq.map Stream.ofArray
                    |> Stream.ofSeq
                    |> Stream.concatStreams)
                |> time "concatStreams"
        assertEqualStreams seqValue streamValue

    [<TestMethod>]
    member __.Builder1() =
        let streamA =
            stream {
                yield 1
                yield! [2; 3]
            }
        let streamB =
            [1; 2; 3] |> Stream.ofSeq
        assertEqualStreams streamA streamB

    [<TestMethod>]
    member __.Builder2() =
        let data = [| 1L..1000000L |]
        let predicate x = (x % 2L = 0L)
        let projection x = x * x
        let streamValue =
            (fun () ->
                data
                    |> Stream.ofArray
                    |> Stream.where predicate
                    |> Stream.map projection)
                |> time "stream"
        let builderValue =
            (fun () ->
                stream {
                    for x in data do
                        if predicate x then
                            yield projection x
                })
                |> time "builder"
        assertEqualStreams streamValue builderValue

    [<TestMethod>]
    member __.Collect() =
        let makeArray n =
            Array.init n id
        let builderValue =
            stream {
                yield! makeArray 1
                yield! makeArray 2
                yield! makeArray 3
            }
        let collectValue =
            [1; 2; 3]
                |> Seq.map makeArray
                |> Stream.ofSeq
                |> Stream.collect Stream.ofArray
        assertEqualStreams builderValue collectValue

    [<TestMethod>]
    member __.Truncate() =
        let stream =
            Stream.init 2 id
                |> Stream.truncate 10
        Assert.AreEqual(2, stream |> Stream.length)
