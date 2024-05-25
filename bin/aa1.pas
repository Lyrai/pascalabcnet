uses System;
uses System.Diagnostics;
//uses System.Collections.Generic;
uses Collections;

procedure Store<T>(val: T);
begin
    var g := val;
end;

type Test<U> = class
private
    arr: array of U;
    size: integer;
    capacity: integer;
    
public 
    asd: integer;
    property Arr1: array of U read arr;
    function GetArr(): array of U;
    begin
        Result := arr;
    end;
    
    constructor Create();
    begin
        arr := new U[2];
        size := 0;
        capacity := 2;
    end;
    
    procedure Add(val: U);
    begin
        if size >= capacity then
        begin
            capacity *= 2;
            var tmp := arr;
            arr := new U[capacity];
            for var i := 0 to tmp.Length - 1 do
            begin
                arr[i] := tmp[i];
            end
        end;
        arr[size] := val;
        size += 1;
    end;
    
    function Get(idx: integer): U;
    begin
        Result := arr[idx];
    end;
end;

type ITest = interface
    procedure test();
end;

type Test1 = class
end;

function ReturnT<T>(tt: T): T;
begin
  Result := tt;
end;

procedure Def<C>(var a: C);
begin
  var cc := new List<C>;
  a := default(C);
end;

begin
  var stack1 := new Stack<integer>;
  var fd := 1;
  writeln(fd);
  Def(fd);
  writeln(fd);
  writeln(ReturnT(50));
  var a := typeof(integer);
  var b := typeof(Dictionary<string, string>);
  var c := typeof(Dictionary<,>);
  var list123 := new List<integer>;
  var lst := new List<Test1>;
  //list123.Add(1);
  var t := new Test<integer>;
  writeln(t.asd);
  t.Add(1);
  t.Add(2);
  t.Add(3);
  t.Add(4);
  list123.Add(1);
  list123.Add(2);
  list123.Add(3);
  list123.Add(4);
  var newlist := list123.Select(x -> x * 2).ToList();
  writeln(newlist[0]);
  writeln(newlist[1]);
  writeln(newlist[2]);
  writeln(newlist[3]);
  writeln(t.Get(0));
  writeln(t.Get(1));
  writeln(t.Get(2));
  writeln(t.Get(3));
  Store(1);
  var hg := t.Arr1;
  writeln(hg[0]);
end.