# Синтаксис языка

## 1. Структура программы

Программа представляет собой последовательность объявлений функций. Порядок объявлений не важен — все функции видны во всей программе.

fn main() {
    let x: int = 42;
    print(x);
}

fn foo(a: int, b: int): int {
    return a + b;
}

## 2. Объявления функций

Каждая функция имеет имя, список параметров, возвращаемый тип и тело — блок инструкций.
fn <идентификатор>(<параметры>): <тип> {
    <инструкции>
}

Функция `main` является точкой входа и не указывает возвращаемый тип:
fn main() {
    <инструкции>
}


## 3. Блоки и инструкции

**Блок** — последовательность инструкций, заключённая в фигурные скобки. Блок создаёт новую область видимости.
**Инструкции** всегда завершаются точкой с запятой `;`, за исключением составных инструкций (`if`, `while`, `for`), которые включают в себя блоки.

### 3.1. Объявление переменной
let <идентификатор>[: <тип>] = <выражение>;

- Квадратные скобки обозначают необязательность указания типа
- Тип может быть выведен из выражения

### 3.2. Присваивание

<идентификатор> = <выражение>;
Левая часть - только идентификатор ранее объявленной переменной.

### 3.3. Возврат из функции

return [<выражение>];

- Для функций с типом `unit` выражение не указывается
- Для остальных типов выражение обязательно

### 3.4. Ветвление if-else

if (<выражение>) {
    <инструкции>
} else {
    <инструкции>
}
Часть `else` необязательна.

### 3.5. Цикл while

while (<выражение>) {
    <инструкции>
}

### 3.6. Цикл for

for (<инициализация>; <выражение>; <присваивание>) {
    <инструкции>
}

- Инициализация - объявление переменной или присваивание
- Условие - выражение, определяющее продолжение цикла
- Обновление - присваивание, выполняемое в конце каждой итерации

### 3.7. Управление циклом

break;
continue;

### 3.8. Выражение как инструкция
- Вызов функции (`input()`, `print(x)`, пользовательские функции)
- Присваивание (уже описано выше)

## 4. Выражения

### 4.1. Приоритет и ассоциативность операторов

| Приоритет | Категория | Операторы | Ассоциативность |
|-----------|-----------|-----------|-----------------|
| 1  | Первичные | вызов функции `()`, группировка `( )` | — |
| 2  | Унарные | `+` `-` `!` | Правая |
| 3  | Возведение в степень | `^` | Правая |
| 4  | Мультипликативные | `*` `/` `%` | Левая |
| 5  | Аддитивные | `+` `-` | Левая |
| 6  | Сравнения | `<` `>` `<=` `>=` | Левая |
| 7  | Равенства | `==` `!=` | Левая |
| 8  | Логическое И | `&&` | Левая |
| 9  | Логическое ИЛИ | `\|\|` | Левая |
| 10 | Присваивание | `=` | Правая |

## 5. Полная EBNF-грамматика

```ebnf
(* Программа *)
program = { function_declaration } ;

(* Объявления функций *)
function_declaration = "fn" , identifier , "(" , [ parameter_list ] , ")"
                     , [ ":" , type ] , block ;
main_function = "fn" , "main" , "(" , ")" , block ;  (* специальный случай *)

parameter_list = parameter , { "," , parameter } ;
parameter      = identifier , ":" , type ;
type           = "int" | "float" | "str" | "unit" ;

(* Блоки и инструкции *)
block = "{" , { statement } , "}" ;

statement = variable_declaration , ";"
          | assignment , ";"
          | function_call , ";"
          | return_statement , ";"
          | if_statement
          | while_statement
          | for_statement
          | "break" , ";"
          | "continue" , ";" ;

variable_declaration = "let" , identifier , [ ":" , type ] , "=" , expression ;
assignment           = identifier , "=" , expression ;
return_statement     = "return" , [ expression ] ;
if_statement         = "if" , "(" , expression , ")" , block , [ "else" , block ] ;
while_statement      = "while" , "(" , expression , ")" , block ;
for_statement        = "for" , "(" , ( variable_declaration | assignment ) , ";"
                       , expression , ";" , assignment , ")" , block ;

(* Выражения *)
expression  = assignment ;
assignment  = logical_or , [ "=" , assignment ] ;
logical_or  = logical_and , { "||" , logical_and } ;
logical_and = equality , { "&&" , equality } ;

equality   = relational , { ( "==" | "!=" ) , relational } ;
relational = additive , { ( "<" | ">" | "<=" | ">=" ) , additive } ;

additive       = multiplicative , { ( "+" | "-" ) , multiplicative } ;
multiplicative = unary , { ( "*" | "/" | "%" ) , unary } ;

unary = [ "+" | "-" | "!" ] , power ;
power = primary , [ "**" , power ] ;  (* правоассоциативная *)

primary = integer_literal
        | float_literal
        | string_literal
        | bool_literal
        | identifier
        | function_call
        | "(" , expression , ")" ;

function_call = identifier , "(" , [ argument_list ] , ")" ;
argument_list = expression , { "," , expression } ;

(* Терминалы определяются в лексической грамматике *)
identifier      = ? см. 01_lexemes.md ? ;
integer_literal = ? см. 01_lexemes.md ? ;
float_literal   = ? см. 01_lexemes.md ? ;
string_literal  = ? см. 01_lexemes.md ? ;
