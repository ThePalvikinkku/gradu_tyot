using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;

namespace gradu_playwright.Testing;
public class BaseSetup : PageTest
{
    // Override context options to enable video recording for all tests
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [SetUp]
    // Start tracing and navigate to the app before each test
    public async Task SetupAsync()
    {
        await Page.GotoAsync("https://coffee-cart.app/");

        await Page.Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }
    // Helper method to retry file operations with delay, to handle cases where files are locked or in use
    private static async Task RetryFileOpAsync(Action op, int attempts = 10, int delayMs = 200)
    {
        for (int i = 0; i < attempts; i++)
        {
            try
            {
                op();
                return;
            }
            catch (IOException) when (i < attempts - 1)
            {
                await Task.Delay(delayMs);
            }
            catch (UnauthorizedAccessException) when (i < attempts - 1)
            {
                await Task.Delay(delayMs);
            }
        }

        op();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        // Determine if the test failed and create a safe filename for artifacts
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var testNameRaw = TestContext.CurrentContext.Test.Name;
        var safeTestName = Regex.Replace(testNameRaw, @"[^\w\-\.]+", "_");

        // Get video path before closing page
        string? originalVideoPath = null;
        if (Page.Video != null)
            originalVideoPath = await Page.Video.PathAsync();

        // Stop tracing before closing
        if (failed)
        {
            await Page.Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = $"trace-{safeTestName}.zip"
            });
        }
        else
        {
            await Page.Context.Tracing.StopAsync(); // discard on pass
        }

        // Close page to flush/finalize video
        await Page.CloseAsync();

        // Handle video
        if (!string.IsNullOrWhiteSpace(originalVideoPath) && File.Exists(originalVideoPath))
        {
            if (failed)
            {
                Directory.CreateDirectory("videos");
                var targetVideoPath = Path.Combine("videos", $"video-{safeTestName}.webm");

                await RetryFileOpAsync(() =>
                {
                    if (File.Exists(targetVideoPath))
                        File.Delete(targetVideoPath);

                    File.Move(originalVideoPath, targetVideoPath);
                });
            }
            else
            {
                await RetryFileOpAsync(() => File.Delete(originalVideoPath));
            }
        }
    }
}