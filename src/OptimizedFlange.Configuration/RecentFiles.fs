namespace OptimizedFlange.Configuration

open System

/// <summary>Represents one entry in the recent project list.</summary>
type RecentFileEntry =
    {
        /// <summary>Absolute project path.</summary>
        Path: string
        /// <summary>Name displayed to the user.</summary>
        DisplayName: string
        /// <summary>Last time the project was opened.</summary>
        LastOpenedAt: DateTimeOffset
        /// <summary>Last known save time.</summary>
        LastSavedAt: DateTimeOffset option
        /// <summary>Whether the file existed when the entry was last refreshed.</summary>
        FileExists: bool
        /// <summary>Known project schema version, when available.</summary>
        ProjectSchemaVersion: int option
        /// <summary>Whether the entry is pinned and exempt from normal eviction.</summary>
        Pinned: bool
    }

/// <summary>Pure operations for maintaining the recent-file list.</summary>
module RecentFiles =
    /// <summary>Maximum number of non-pinned recent project entries.</summary>
    [<Literal>]
    let MaxRecentFiles = 20

    /// <summary>Adds or refreshes an entry while preserving pinned entries and capping non-pinned entries at twenty.</summary>
    let addOrUpdate (entry: RecentFileEntry) (items: RecentFileEntry list) =
        let withoutSamePath =
            items
            |> List.filter (fun item ->
                not (String.Equals(item.Path, entry.Path, StringComparison.OrdinalIgnoreCase)))

        let merged = entry :: withoutSamePath
        let pinned = merged |> List.filter (fun item -> item.Pinned)
        let recent =
            merged
            |> List.filter (fun item -> not item.Pinned)
            |> List.sortByDescending (fun item -> item.LastOpenedAt)
            |> List.truncate MaxRecentFiles

        pinned @ recent
