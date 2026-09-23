using System.Text;
using Bunit;
using Shouldly;
using SyntaxCircus.Blazor.Components.Interop;
using Xunit;

namespace SyntaxCircus.Blazor.Components.Tests;

public sealed class FileDownloadServiceTests
{
    private const string ModulePath = "./_content/SyntaxCircus.Blazor.Components/fileDownload.js";

    [Fact]
    public async Task DownloadAsync_WithBytes_InvokesModuleWithFileNameAndContentType()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("downloadFile", _ => true).SetVoidResult();

        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await sut.DownloadAsync(
            "report.pdf",
            "application/pdf",
            Encoding.UTF8.GetBytes("hello"),
            Xunit.TestContext.Current.CancellationToken);

        var invocation = module.VerifyInvoke("downloadFile");
        invocation.Arguments[0].ShouldBe("report.pdf");
        invocation.Arguments[1].ShouldBe("application/pdf");
    }

    [Fact]
    public async Task DownloadAsync_WithStream_InvokesModuleWithFileNameAndContentType()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("downloadFile", _ => true).SetVoidResult();

        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("hello"));

        await sut.DownloadAsync(
            "export.json", "application/json", content, Xunit.TestContext.Current.CancellationToken);

        var invocation = module.VerifyInvoke("downloadFile");
        invocation.Arguments[0].ShouldBe("export.json");
        invocation.Arguments[1].ShouldBe("application/json");
    }

    [Fact]
    public async Task DownloadAsync_WithStream_LeavesCallerStreamOpen()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("downloadFile", _ => true).SetVoidResult();

        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("hello"));

        await sut.DownloadAsync(
            "export.json", "application/json", content, Xunit.TestContext.Current.CancellationToken);

        Should.NotThrow(() => content.Position = 0);
    }

    [Fact]
    public async Task DownloadAsync_WithEmptyContentType_FallsBackToOctetStream()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("downloadFile", _ => true).SetVoidResult();

        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await sut.DownloadAsync(
            "report.bin", string.Empty, [1, 2, 3], Xunit.TestContext.Current.CancellationToken);

        var invocation = module.VerifyInvoke("downloadFile");
        invocation.Arguments[1].ShouldBe("application/octet-stream");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task DownloadAsync_WithEmptyFileName_ThrowsArgumentException(string? fileName)
    {
        using var context = new BunitContext();
        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await Should.ThrowAsync<ArgumentException>(
            () => sut.DownloadAsync(fileName!, "application/octet-stream", [1, 2, 3]));
    }

    [Fact]
    public async Task DownloadAsync_WithNullBytes_ThrowsArgumentNullException()
    {
        using var context = new BunitContext();
        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await Should.ThrowAsync<ArgumentNullException>(
            () => sut.DownloadAsync("report.pdf", "application/pdf", (byte[])null!));
    }

    [Fact]
    public async Task DownloadAsync_WithNullStream_ThrowsArgumentNullException()
    {
        using var context = new BunitContext();
        await using var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await Should.ThrowAsync<ArgumentNullException>(
            () => sut.DownloadAsync("report.pdf", "application/pdf", (Stream)null!));
    }

    [Fact]
    public async Task DisposeAsync_AfterDownload_DisposesTheImportedModuleWithoutThrowing()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.SetupVoid("downloadFile", _ => true).SetVoidResult();

        var sut = new FileDownloadService(context.JSInterop.JSRuntime);
        await sut.DownloadAsync(
            "report.pdf", "application/pdf", [1, 2, 3], Xunit.TestContext.Current.CancellationToken);

        await Should.NotThrowAsync(() => sut.DisposeAsync().AsTask());
    }

    [Fact]
    public async Task DisposeAsync_WithoutADownload_DoesNotImportTheModule()
    {
        using var context = new BunitContext();
        context.JSInterop.SetupModule(ModulePath);

        var sut = new FileDownloadService(context.JSInterop.JSRuntime);

        await Should.NotThrowAsync(() => sut.DisposeAsync().AsTask());
    }
}
