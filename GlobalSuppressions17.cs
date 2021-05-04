// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S2933:Fields that are only assigned in the constructor should be \"readonly\"", Justification = "<Pending>", Scope = "member", Target = "~F:kcp2k.KcpServer.connectionsToRemove")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.#ctor(System.Action{System.Int32},System.Action{System.Int32,System.ArraySegment{System.Byte}},System.Action{System.Int32},System.Boolean,System.UInt32,System.Int32,System.Boolean,System.UInt32,System.UInt32)")]
[assembly: SuppressMessage("Style", "IDE0017:Simplify object initialization", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.Start(System.UInt16)")]
[assembly: SuppressMessage("Info Code Smell", "S1135:Track uses of \"TODO\" tags", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.TickIncoming")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.TickIncoming")]
[assembly: SuppressMessage("Major Code Smell", "S108:Nested blocks of code should not be left empty", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.TickIncoming")]
[assembly: SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>", Scope = "member", Target = "~F:kcp2k.KcpServer.connectionsToRemove")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:kcp2k.KcpServer.GetClientAddress(System.Int32)~System.String")]
