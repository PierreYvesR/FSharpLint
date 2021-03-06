module FSharpLint.Core.Tests.Rules.Conventions.NoPartialFunctions

open NUnit.Framework
open FSharpLint.Rules

[<TestFixture>]
type TestConventionsNoPartialFunctions() =
    inherit TestAstNodeRuleBase.TestAstNodeRuleBase(NoPartialFunctions.rule { AdditionalPartials = ["Custom.partial"]; AllowedPartials = ["List.pick"] })

    [<Test>]
    member this.``Error for partial function which should be replaced with pattern matching``() =
        this.Parse("let x = Option.get None")

        this.AssertErrorWithMessageExists("Consider using pattern matching instead of partial function 'Option.get'.")

    [<Test>]
    member this.``Error for partial function which should be replaced with another function``() =
        this.Parse("let x = List.find 1 [2; 3; 4]")

        this.AssertErrorWithMessageExists("Consider using 'List.tryFind' instead of partial function 'List.find'.")

    [<Test>]
    member this.``Quickfix for partial function which should be replaced with another function``() =
        let source = "let x = List.find 1 [2; 3; 4]"
        this.Parse(source)

        let expected = "let x = List.tryFind 1 [2; 3; 4]"
        Assert.AreEqual(expected, this.ApplyQuickFix source)
        this.AssertErrorWithMessageExists( "Consider using 'List.tryFind' instead of partial function 'List.find'.")

    [<Test>]
    member this.``Error for user-specified partial function``() =
        this.Parse("let x = Custom.partial 4")

        this.AssertErrorWithMessageExists("Consider not using partial function 'Custom.partial'.")

    [<Test>]
    member this.``No error for user-specified allowed partial function``() =
        this.Parse("let x = List.pick id [Some 4; None]")

        this.AssertNoWarnings()
