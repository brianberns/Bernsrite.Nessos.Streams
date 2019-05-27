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

    [<TestMethod>]
    member __.GroupByStreams() =
        let data = [| 1L..1000000L |]
        let seqValue =
            (fun () ->
                data
                    |> Stream.ofArray
                    |> Stream.groupBy (fun x -> x % 2L)
                    |> Stream.map (fun (x, grouping) -> x, Seq.sum grouping)
                    |> Stream.toSeq
                    |> set)
                |> time "groupBy"
        let streamValue =
            (fun () ->
                data
                    |> Stream.ofArray
                    |> Stream.groupByStreams (fun x -> x % 2L)
                    |> Stream.map (fun (x, grouping) -> x, Stream.sum grouping)
                    |> Stream.toSeq
                    |> set)
                |> time "groupByStreams"
        Assert.AreEqual(seqValue, streamValue)
