﻿/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: AssemblyTitle("db4o - optional functionality")]
[assembly: AssemblyCompany("Versant Inc., San Mateo, CA, USA")]
[assembly: AssemblyProduct("db4o - database for objects")]
[assembly: AssemblyCopyright("db4o 2005 - 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// attributes are automatically set by the build
[assembly: AssemblyVersion("7.11.107.13460")]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyConfiguration(".NET")]
[assembly: AssemblyDescription("Db4objects.Db4o.Optional 7.11.107.13460 (.NET)")]

#if !CF && !SILVERLIGHT
[assembly: AllowPartiallyTrustedCallers]
#endif