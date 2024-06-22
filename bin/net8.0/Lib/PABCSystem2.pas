// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/// Стандартный модуль
/// !! System unit
unit PABCSystem;

{$apptype console}

{$reference '%GAC%\System.Private.CoreLib.dll'}
{$reference '%GAC%\System.dll'}
{$reference '%GAC%\System.Text.RegularExpressions.dll'}
{$reference '%GAC%\System.Core.dll'}
{$reference '%GAC%\System.Runtime.Numerics.dll'}
{$reference '%GAC%\System.Console.dll'}
{$reference '%GAC%\System.Collections.dll'}
{$reference '%GAC%\System.Linq.dll'}
{$reference '%GAC%\System.Runtime.Serialization.Formatters.dll'}
{$reference '%GAC%\System.IO.FileSystem.DriveInfo.dll'}
{$reference '%GAC%\System.Diagnostics.Process.dll'}

interface

uses
  System.Runtime.InteropServices,
  System.IO, 
  System.Collections, 
  System.Collections.Generic,
  System;

{uses System.Collections.Generic;
uses System;

var
  __CONFIG__: Dictionary<string, object> := new Dictionary<string, object>;}

function RuntimeDetermineType(T: System.Type): byte;
function RuntimeInitialize(kind: byte; variable: object): object;
type TypedSet = class
end;
type Text = class
end;
type BinaryFile = class
end;

implementation

function RuntimeDetermineType(T: System.Type): byte;
begin
  result := 0;
  if T.IsValueType and (T.GetMethod('$Init$') <> nil) then
  begin
    result := 1;
    exit;
  end;
  if T = typeof(string) then
  begin
    result := 2;
    exit;
  end;
  if T = typeof(TypedSet) then
  begin
    result := 3;
    exit;
  end;
  if T = typeof(Text) then
  begin
    result := 4;
    exit;
  end;
  if T = typeof(BinaryFile) then
  begin
    result := 5;
    exit;
  end;
end;

function RuntimeInitialize(kind: byte; variable: object): object;
begin
  case kind of
    1: 
      begin
        {variable.GetType.InvokeMember('$Init$',
        System.Reflection.BindingFlags.InvokeMethod or
        System.Reflection.BindingFlags.Instance or
        System.Reflection.BindingFlags.Public, nil, variable, nil);
        result := variable;}
      end;
    2: result := '';
    3: result := new TypedSet;
    4: result := new Text;
    5: result := new BinaryFile;
  end;
end;

initialization
 
finalization
  
end.