# Тесты EntryPointTest

### Can_exec_main - Проверка наличия main и возвращаемого exitCode
| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| exitCode 0 | `fn main(): int { print("Program exited with code 0"); return 0; }` | `"Program exited with code 0", 0` |
| exitCode 1 | `fn main(): int { print("Program exited with code 1"); return 1; }` | `"Program exited with code 1", 1` |

### Throws_on_invalid_entry_point_declaration - Ошибки при неверном объявлении main

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Без точки входа | `printi(1);` | `UnexpectedLexemeException` |
| Тип unit вместо int | `fn main(): unit { printi(0); }` | `UnexpectedLexemeException` |
| Тип str вместо int | `fn main(): int { printi(0); return ""; }` | `TypeErrorException` |
| Тип float вместо int | `fn main(): int { printi(0); return 0.0; }` | `TypeErrorException ` |
| Тип unit вместо int | `fn main(): int { printi(0); return; }` | `TypeErrorException ` |
| Отсутствие return | `fn main(): int { printi(0); }` | `TypeErrorException` |
| Недостижимый код после return | `fn main(): int { return 1; printi(0); }` | `InvalidOperationException` |

# Тесты BuiltinFunctionsTest

### Can_evaluate_builtin_functions - Функции преобразования типов

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Преобразование строки в целое число | `printi(stoi("5"));` | `Value(5)` |
| Преобразование целого числа в строку | `print(itos(5));` | `Value("5")` |
| Преобразование целого числа в число с плавающей точкой | `printf(itof(49), 1);` | `Value(49.0)` |
| Преобразование числа с плавающей точкой в целое число | `printi(ftoi(49.0));` | `Value(49)` |
| Преобразование строки в число с плавающей точкой | `printf(stof("49.0"), 1);` | `Value(49.0)` |
| Преобразование числа с плавающей точкой в строку с указанной точностью | `print(ftos(49.14, 2));` | `Value("49.14")` |
| Подсчет длины строки | `printi(strlen("Hello!"));` | `Value(6)` |
| Извлечение подстроки | `print(substr("Hello!", 2, 2));` | `Value("ll")` |
| Извлечение подстроки до конца | `print(substr("Hello!", 2, 4));` | `Value("llo!")` |
| Конкатенация двух строк | `print(sconcat("Ali", "ce"));` | `Value("Alice")` |

### Can_evaluate_output_functions - Функции вывода

| Тест-кейс | Код | Ожидаемый результат | Ожидаемый буфер | Ожидаемый сброс |
|-----------|-----|---------------------|-----------------|-----------------|
| Вывод строки | `print("Hello!");` | `Value.Unit` | `"Hello!"` | `""` |
| Вывод результата целочисленного выражения | `printi(2 + 7);` | `Value.Unit` | `"9"` | `""` |
| Множественные операторы вывода | `printi(2 + 7); print("\n"); printi(2 - 7); print("\n");` | `Value.Unit` | `"9\n-5\n"` | `""` |
| Смешанный вывод строк и чисел | `printi(7); print("\n"); printi(4);` | `Value.Unit` | `"7\n4"` | `""` |

### Can_evaluate_input_functions - Функции ввода

| Тест-кейс | Код | Ввод | Ожидаемый буфер |
|-----------|-----|------|-----------------|
| Вывод результата функции ввода | `print(input());` | `"x"` | `"x"` |

### Throws_on_invalid_function_calls - Неверные вызовы встроенных функций

| Тест-кейс | Код | Ожидаемое исключение |
|-----------|-----|---------------------|
| Вызов неизвестной функции | `length("Hello!");` | `UnknownSymbolException` |
| Передача целого числа в функцию strlen | `strlen(10);` | `TypeErrorException` |
| Вызов strlen с двумя аргументами | `strlen("Hello!", "World");` | `InvalidFunctionCallException` |
| Вызов strlen без аргументов | `strlen();` | `InvalidFunctionCallException` |
| Вызов sconcat без аргументов | `sconcat();` | `InvalidFunctionCallException` |
| Вызов sconcat с одним аргументом | `sconcat("a");` | `InvalidFunctionCallException` |
| Вызов sconcat с тремя аргументами | `sconcat("a", "b", "c");` | `InvalidFunctionCallException` |

# Тесты ExpressionsTest

### Арифметические выражения

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Умножение вещественных чисел | `3.14 * 2;` | `Value(6.28)` |
| Степень и унарный минус | `-5 ** 2;` | `Value(-25)` |
| Степень и унарный минус с приоритетом | `(-5) ** 2;` | `Value(25)` |
| Модуль и возведение в степень | `4 % 3 ** 2;` | `Value(4)` |
| Модуль и возведение в степень с приоритетом | `(4 % 3) ** 2;` | `Value(1)` |
| Приоритет операций (сложение/умножение) | `1 + 2 * 8 / 3 - 1;` | `Value(5)` |
| Приоритет операций со скобками | `(1 + 2) * (8 / (3 - 1));` | `Value(12)` |
| Правоассоциативность возведения в степень | `2 ** 3 ** 2;` | `Value(512)` |
| Левоассоциативность вычитания | `10 - 3 - 2;` | `Value(5)` |
| Левоассоциативность деления | `10 / 3 / 2;` | `Value(1)` |
| Левоассоциативность смешанных операций | `10 - 3 + 2;` | `Value(9)` |
| Левоассоциативность смешанных операций с делением | `10 / 3 * 2;` | `Value(6)` |

### Унарные операторы

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Унарный оператор + | `printi(+1)` | `Value(1)` |
| Унарный оператор - | `printi(-4);` | `Value(-4)` |
| Унарный - в выражении | `printi(2 * 2 * -5);` | `Value(-20)` |
| Унарный - вместе с бинарным + | `printi(1+-1);` | `Value(0)` |
| Унарный + вместе с бинарным + | `printi(1++1);` | `Value(2)` |
| Двойное отрицание | `printi(-(-5));` | `Value(5)` |
| Унарный - и унарный + | `printi(+(-5));` | `Value(-5)` |

### Неверные выражения

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Последовательный унарный + | `printi(++1);` | `UnexepectedLexemeException`   |
| Последовательный унарный - | `printi(--1);` | ``UnexepectedLexemeException`` |
| Последовательный унарный + после литерала | `printi(1++)` | `UnexepectedLexemeException` |
| Последовательный унарный - после литерала | `printi(1--)` | `UnexepectedLexemeException` |
| Комбинация унарных операторов | `printi(-+1);` | `UnexepectedLexemeException` |
| Комбнации унарных операторов в бинарных выражениях | `printi(1+++1); printi(1---1); printi(1+-+1); printi(1-+-1);` | `UnexepectedLexemeException` |
| Незакрытая скобка в выражении | `printi(((5 + 5) * 2);` | `UnexepectedLexemeException` |


# Тесты VariablesTest

| Тест-кейс | Код | Ожидаемый результат |
|-----------|-----|---------------------|
| Объявление int | `let x = 0; printi(x);` | `0` |
| Объявление str | `let x = "Hello World"; print(x);` | `Hello World` |
| Объявление float | `let x = 3.14; printf(x, 2);` | `3.14` |
| Присваивание нового значения | `let x = 3.14; x = 0.0; printf(x, 1);` | `0.0` |
