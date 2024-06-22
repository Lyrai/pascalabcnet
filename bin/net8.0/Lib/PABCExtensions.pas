// Copyright (c) Ivan Bondarev, Stanislav Mikhalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
///--
unit PABCExtensions;

{$zerobasedstrings off}

uses PABCSystem;

function GetCurrentLocale: string;
begin
  var locale: object;
  if __CONFIG__.TryGetValue('locale', locale) then
    Result := locale as string
  else
    Result := 'ru';
end;

function GetTranslation(message: string): string;
begin
  var cur_locale := GetCurrentLocale();
  var arr := message.Split(new string[1]('!!'), System.StringSplitOptions.None);
  if (cur_locale = 'en') and (arr.Length > 1) then
    Result := arr[1]
  else
    Result := arr[0]
end;

const
  BAD_TYPE_IN_TYPED_FILE = 'Для типизированных файлов нельзя указывать тип элементов, являющийся ссылочным или содержащий ссылочные поля!!Typed file cannot contain elements that are references or contains fields-references';
  PARAMETER_STEP_MUST_BE_NOT_EQUAL_0 = 'Параметр step не может быть равен 0!!The step parameter must be not equal to 0';
  PARAMETER_FROM_OUT_OF_RANGE = 'Параметр from за пределами диапазона!!The from parameter out of bounds';
  PARAMETER_TO_OUT_OF_RANGE = 'Параметр to за пределами диапазона!!The to parameter out of bounds';
  SLICE_SIZE_AND_RIGHT_VALUE_SIZE_MUST_BE_EQUAL = 'Размеры среза и присваиваемого выражения должны быть равны!!Slice size and assigned expression size must be equal';

//{{{doc: Начало секции расширений строк для срезов }}} 

///--
procedure CorrectFromTo(situation: integer; Len: integer; var from, &to: integer; step: integer);
begin
  if step > 0 then
  begin
    case situation of
      1: from := 0;
      2: &to := Len;
      3: (from, &to) := (0, Len)
    end;  
  end
  else
  begin
    case situation of
      1: from := Len - 1;
      2: &to := -1;
      3: (from, &to) := (Len - 1, -1);
    end;
  end;
end;

///--
function CheckAndCorrectFromToAndCalcCountForSystemSlice(situation: integer; Len: integer; var from, &to: integer; step: integer): integer;
begin
  // situation = 0 - все параметры присутствуют
  // situation = 1 - from отсутствует
  // situation = 2 - to отсутствует
  // situation = 3 - from и to отсутствуют
  if step = 0 then
    raise new System.ArgumentException(GetTranslation(PARAMETER_STEP_MUST_BE_NOT_EQUAL_0));
  
  if (situation = 0) or (situation = 2) then
    if (from < 0) or (from > Len - 1) then
      raise new System.ArgumentException(GetTranslation(PARAMETER_FROM_OUT_OF_RANGE));
  
  if (situation = 0) or (situation = 1) then
    if (&to < -1) or (&to > Len) then
      raise new System.ArgumentException(GetTranslation(PARAMETER_TO_OUT_OF_RANGE));
  
  CorrectFromTo(situation, Len, from, &to, step);
  
  var count: integer;
  
  if step > 0 then
  begin
    var cnt := &to - from;
    if cnt <= 0 then 
      count := 0
    else count := (cnt - 1) div step + 1;
  end
  else
  begin
    var cnt := from - &to;
    if cnt <= 0 then 
      count := 0
    else count := (cnt - 1) div (-step) + 1;
  end;
  
  Result := count;
end;

procedure SystemSliceAssignmentStringImpl(var Self: string; rightValue: string; situation: integer; 
  from, &to: SystemIndex; step: integer; baseIndex: integer := 1);
begin
  from.IndexValue := from.IndexValue - baseIndex;
  &to.IndexValue := &to.IndexValue - baseIndex;
  
  var fromValue := from.IndexValue;
  var toValue := &to.IndexValue;
  var count := CheckAndCorrectFromToAndCalcCountForSystemSlice(situation, Self.Count, fromValue, toValue, step);
  if count <> rightValue.Length then
    raise new System.ArgumentException(GetTranslation(SLICE_SIZE_AND_RIGHT_VALUE_SIZE_MUST_BE_EQUAL));
    
  var f := fromValue + 1;
  
  var strInd := 1;
  loop count do
  begin
    //Self[f] := rightValue[strInd];
    f += step;
    strInd += 1;
  end;
end;  

///--
procedure SystemSliceAssignment(var Self: string; rightValue: string; situation: integer; from, &to: integer; 
  step: integer := 1); extensionmethod;
begin
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, step);
end;

///--
{procedure SystemSliceAssignment(var Self: string; rightValue: string; situation: integer; from, &to: integer); extensionmethod;
begin
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, 1);
end;}

///--
procedure SystemSliceAssignment(var Self: string; rightValue: string; situation: integer; from, &to: SystemIndex; 
  step: integer := 1); extensionmethod;
begin
  if from.IsInverted then
    from.IndexValue := Self.Length - from.IndexValue + 1;
  if &to.IsInverted then
    &to.IndexValue := Self.Length - &to.IndexValue + 1;
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, step);
end;

///--
{procedure SystemSliceAssignment(var Self: string; rightValue: string; situation: integer; from, &to: SystemIndex); extensionmethod;
begin
  if from.IsInverted then
    from.IndexValue := Self.Length - from.IndexValue + 1;
  if &to.IsInverted then
    &to.IndexValue := Self.Length - &to.IndexValue + 1;
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, 1);
end;}

///--
procedure SystemSliceAssignment0(var Self: string; rightValue: string; situation: integer; from, &to: integer; 
  step: integer := 1); extensionmethod;
begin
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, step, 0);
end;

///--
{procedure SystemSliceAssignment0(var Self: string; rightValue: string; situation: integer; from, &to: integer); extensionmethod;
begin
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, 1, 0);
end;}

///--
procedure SystemSliceAssignment0(var Self: string; rightValue: string; situation: integer; from, &to: SystemIndex; 
  step: integer := 1); extensionmethod;
begin
  if from.IsInverted then
    from.IndexValue := Self.Length - from.IndexValue;
  if &to.IsInverted then
    &to.IndexValue := Self.Length - &to.IndexValue;
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, step, 0);
end;

///--
{procedure SystemSliceAssignment0(var Self: string; rightValue: string; situation: integer; from, &to: SystemIndex); extensionmethod;
begin
  if from.IsInverted then
    from.IndexValue := Self.Length - from.IndexValue;
  if &to.IsInverted then
    &to.IndexValue := Self.Length - &to.IndexValue;
  SystemSliceAssignmentStringImpl(Self, rightValue, situation, from, &to, 1, 0);
end;}

//{{{--doc: Конец секции расширений строк для срезов }}} 

//{{{doc: Начало секции подпрограмм для типизированных файлов для документации }}} 

// -----------------------------------------------------
//>>     Подпрограммы для работы с типизированными и бестиповыми файлами # Subroutines for typed and untyped files
// -----------------------------------------------------


function ContainsReferenceTypes(t: System.Type): boolean;
begin
  if t.IsPrimitive then
    Result := False
  else if t.IsValueType then 
  begin
    var fa := t.GetFields(System.Reflection.BindingFlags.GetField or System.Reflection.BindingFlags.Instance or System.Reflection.BindingFlags.Public or System.Reflection.BindingFlags.NonPublic);
    Result := fa.Any(x->ContainsReferenceTypes(x.FieldType));
  end
  else Result := True;
end;


//{{{--doc: Конец секции подпрограмм для типизированных файлов для документации }}} 

// -----------------------------------------------------
//>>     Функции, создающие HashSet и SortedSet по встроенным множествам # Function for creation HashSet and SortedSet from set of T
// -----------------------------------------------------

{/// Создает HashSet по встроенному множеству
function HSet<T>(s: set of T): HashSet<T>;
begin
  Result := new HashSet<T>;
  foreach var x in s do
    Result += x;
end;

/// Создает SortedSet по встроенному множеству
function SSet<T>(s: set of T): SortedSet<T>;
begin
  Result := new SortedSet<T>;
  foreach var x in s do
    Result += x;
end;}


//------------------------------------------------------------------------------
//          Операции для procedure
//------------------------------------------------------------------------------
///--
function operator*(p: procedure; n: integer): procedure; extensionmethod;
begin
  Result := () -> for var i:=1 to n do p
end;

///--
function operator*(n: integer; p: procedure): procedure; extensionmethod;
begin
  Result := () -> for var i:=1 to n do p
end;

var __initialized: boolean;

{procedure __InitModule;
begin
end;}

procedure __InitModule__;
begin
  if not __initialized then
  begin
    __initialized := true;
    __InitPABCSystem;
    //__InitModule;
  end;
end;

begin
  //__InitModule;
end.