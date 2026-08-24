namespace OptimizedFlange.UnitTests

open System
open OptimizedFlange.Configuration
open Xunit

module RecentFilesTests =
    let private entry index openedAt pinned =
        {
            Path = $"C:\\projects\\joint-{index}.ofj"
            DisplayName = $"joint-{index}"
            LastOpenedAt = openedAt
            LastSavedAt = None
            FileExists = true
            ProjectSchemaVersion = Some 1
            Pinned = pinned
        }

    [<Fact>]
    let ``addOrUpdate caps non pinned entries at twenty and preserves pinned entries`` () =
        let baseTime = DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero)
        let items =
            [ 1..25 ]
            |> List.map (fun index -> entry index (baseTime.AddMinutes(float index)) false)

        let pinned = entry 100 (baseTime.AddMinutes(-1.0)) true
        let actual = RecentFiles.addOrUpdate pinned items

        let nonPinnedCount = actual |> List.filter (fun item -> not item.Pinned) |> List.length

        Assert.Contains(actual, fun item -> item.Path = pinned.Path && item.Pinned)
        Assert.Equal(RecentFiles.MaxRecentFiles, nonPinnedCount)

    [<Fact>]
    let ``addOrUpdate refreshes matching path case insensitively`` () =
        let oldEntry = entry 1 (DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero)) false
        let refreshed =
            {
                oldEntry with
                    Path = oldEntry.Path.ToUpperInvariant()
                    DisplayName = "refreshed"
                    LastOpenedAt = oldEntry.LastOpenedAt.AddHours(1.0)
            }

        let actual = RecentFiles.addOrUpdate refreshed [ oldEntry ]

        Assert.Single(actual) |> ignore
        Assert.Equal("refreshed", actual.Head.DisplayName)
