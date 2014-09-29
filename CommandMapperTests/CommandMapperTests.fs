[<NUnit.Framework.TestFixture>]
module CommandMapperTests 

open NUnit.Framework
open FsUnit
open HyperLoop

type private Tester() =
    let mutable _lastCall = 0
    member __.function1() =
        _lastCall <- 1
    member __.function2() =
        _lastCall <- 2
    member __.LastCall = _lastCall

[<Test>]
let ``Supported commands are called``() = 
    let tester = Tester()
    let mappings = 
        [| 
            ({CommandString="1"; Function=tester.function1})
            ({CommandString="2"; Function=tester.function2})
        |]
    let mapper = CommandMapper(mappings)

    for i in [1; 2] do
        let expected = i
        mapper.Do (i.ToString())
        let actual = tester.LastCall
        actual |> should equal expected

[<Test>]
let ``Unsupported commands cause an exception``() = 
    let tester = Tester()
    let mappings = 
        [| 
            ({CommandString="1"; Function=tester.function1})
            ({CommandString="2"; Function=tester.function2})
        |]
    let mapper = CommandMapper(mappings)

    (fun () -> mapper.Do "3") |> should throw typeof<System.ArgumentException>


