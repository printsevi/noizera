// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Classes in the Configurations folder are used by EF Core through reflection",
    Scope = "namespaceanddescendants",
    Target = "~N:Noizera.Common.Persistence.SQL.Configurations")]
[assembly: SuppressMessage("Style", "IDE0161:Convert to file-scoped namespace", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Style", "IDE0053:Use expression body for lambda expression", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Style", "IDE0005:Using directive is unnecessary.", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Style", "IDE0053:Use expression body for lambda expression", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "~N:Noizera.Common.Persistence.Migrations")]
