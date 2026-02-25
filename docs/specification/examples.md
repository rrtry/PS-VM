# Примеры программ на языке

## Общие примеры

### Типы данных и функции преобразования

```
fn data_type_check() {
    let x: int = 42;
    let y: float = 3.14;
    let sum = x + y;
    
    print(itos(123));
    print(ftos(3.14159, 2));
    printi(ftoi(3.9));
    printi(stoi("456"));
}
```

### Операторы

```
fn operators_check() {
    let a = 10;
    let b = 3;
    
    printi(a + b); 
    printi(a - b); 
    printi(a * b);
    printi(a / b); 
    printi(a % b); 
    printi(2 ** 3);
    printi(-a); 
    printi(!true);
}
```

### Цикл For и управление им

```
fn for_loop_check() {
    let mut i = 0;
    for (i = 1; i <= 5; i = i + 1) {
        printi(i);
    }
    
    let mut n = 1;
    while (n < 100) {
        if (n > 10) { break; }
        n = n * 2;
    }
}
```

### Проверка области видимостей

```
let global = 100;

fn scope_check() {
    let local = 10;
    {
        let local = 20;
        printi(local);
    }
    printi(local);
    printi(global);
}
```

### Операции над строками

```
fn string_check() {
    let s = sconcat("Hello", " World");
    let len = strlen(s);
    let sub = substr(s, 0, 5);
    
    let esc = "Кавычки: \" и \\ и \n";
}
```

### Проверка граничных случаев

```
fn boundary_check() {
    safe_div(10, 0);
    
    let max = 9223372036854775807;
    let empty = "";
    let len = strlen(empty);
}

fn safe_div(a: int, b: int): unit {
    if (b == 0) {
        print("Деление на 0!\n");
        return;
    }
    printi(a / b);
}
```

## Явные примеры программ

### 1. GCD — наибольший общий делитель (алгоритм Евклида)

```
fn main() {
    let a = stoi(input());
    let b = stoi(input());

    let result = gcd(a, b);

    print("НОД = ");
    printi(result);
    print("\n");
}

fn gcd(mut x: int, mut y: int): int {
    while (y != 0) {
        let temp = y;
        y = x % y;
        x = temp;
    }
    return x;
}
```

---

### 2. QuadraticEquation — решение квадратного уравнения ax² + bx + c = 0

```
fn main() {
    let a = stof(input());
    let b = stof(input());
    let c = stof(input());

    solve(a, b, c);
}

fn solve(a: float, b: float, c: float): unit {
    if (a == 0.0) {
        if (b == 0.0) {
            print("Уравнение вырожденное\n");
            return;
        }
        let x = -c / b;
        print("Один корень: ");
        print(ftos(x, 4));
        print("\n");
        return;
    }

    let d = b * b - 4.0 * a * c;

    if (d < 0.0) {
        print("Нет действительных корней\n");
        return;
    }

    if (d == 0.0) {
        let x = -b / (2.0 * a);
        print("Один корень (кратный): ");
        print(ftos(x, 4));
        print("\n");
        return;
    }

    let sd = d ** 0.5;
    let x1 = (-b + sd) / (2.0 * a);
    let x2 = (-b - sd) / (2.0 * a);

    print("Два корня: ");
    print(ftos(x1, 4));
    print("  ");
    print(ftos(x2, 4));
    print("\n");
}
```

---

### 3. ReverseString — реверс строки

```
fn main() {
    let s = input();
    let rev = reverse(s);
    print(rev);
    print("\n");
}

fn reverse(s: str): str {
    let len = strlen(s);
    let result = "";
    let mut i = 0;

    for (i = len - 1; i >= 0; i = i - 1) {
        result = sconcat(result, substr(s, i, 1));
    }

    return result;
} 
```

### 4. CheckPalindrome — проверка, является ли строка палиндромом (с учётом регистра и пробелов)

```
fn main() {
    let line = input();
    let cleaned = normalize(line);

    if (is_palindrome(cleaned)) {
        print("yes\n");
    } else {
        print("no\n");
    }
}

fn normalize(s: str): str {
    let len = strlen(s);
    let result = "";
    let i = 0;

    while (i < len) {
        let ch = substr(s, i, 1);
        let lower = to_lower(ch);

        if ((lower >= "a" && lower <= "z") || (lower >= "а" && lower <= "я")) {
            result = sconcat(result, lower);
        }

        i = i + 1;
    }

    return result;
}

fn to_lower(ch: str): str {
    if (ch == "A") { return "a"; }
    if (ch == "B") { return "b"; }
    if (ch == "C") { return "c"; }
    if (ch == "D") { return "d"; }
    if (ch == "E") { return "e"; }
    if (ch == "F") { return "f"; }
    if (ch == "G") { return "g"; }
    if (ch == "H") { return "h"; }
    if (ch == "I") { return "i"; }
    if (ch == "J") { return "j"; }
    if (ch == "K") { return "k"; }
    if (ch == "L") { return "l"; }
    if (ch == "M") { return "m"; }
    if (ch == "N") { return "n"; }
    if (ch == "O") { return "o"; }
    if (ch == "P") { return "p"; }
    if (ch == "Q") { return "q"; }
    if (ch == "R") { return "r"; }
    if (ch == "S") { return "s"; }
    if (ch == "T") { return "t"; }
    if (ch == "U") { return "u"; }
    if (ch == "V") { return "v"; }
    if (ch == "W") { return "w"; }
    if (ch == "X") { return "x"; }
    if (ch == "Y") { return "y"; }
    if (ch == "Z") { return "z"; }    
    
    return ch;
}

fn is_palindrome(s: str): bool {
    let len = strlen(s);
    let left = 0;
    let right = len - 1;

    while (left < right) {
        if (substr(s, left, 1) != substr(s, right, 1)) {
            return false;
        }
        left = left + 1;
        right = right - 1;
    }

    return true;
}
```

### 5. Программа IsPrime - Вычисление простого числа

```
fn main() 
{
    let result = is_prime(5);
    printi(result);
}

fn is_prime(n): int
{
    if (n < 2) 
    {
        return 0;
    }
    if (n == 2) 
    {
        return 1;
    }

    let limit = sqrt(n);
    let i = 3;

    while (i <= limit) 
    {
        if (n % i == 0) 
        {
            return 0;
        }
        i = i + 2;
    }
    return 1;
}
```