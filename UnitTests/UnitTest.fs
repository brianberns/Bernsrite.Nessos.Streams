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

    let assertEqualStreams streamA streamB =
        let arrayA = streamA |> Stream.toArray
        let arrayB = streamB |> Stream.toArray
        Assert.AreEqual(arrayA.Length, arrayB.Length)
        for (a, b) in Seq.zip arrayA arrayB do
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
