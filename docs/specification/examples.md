# Примеры программ на языке

## 1. GCD — наибольший общий делитель (алгоритм Евклида)
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

## 2. QuadraticEquation — решение квадратного уравнения ax² + bx + c = 0
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

## 3. ReverseString — реверс строки
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

    let i = len - 1;
    while (i >= 0) {
        result = sconcat(result, substr(s, i, 1));
        i = i - 1;
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

        // оставляем только буквы (очень упрощённо)
        if ((lower >= "a" && lower <= "z") || (lower >= "а" && lower <= "я")) {
            result = sconcat(result, lower);
        }

        i = i + 1;
    }

    return result;
}

fn to_lower(c: str): str {
    if (c == "A") { return "a"; }
    if (c == "B") { return "b"; }
    if (c == "C") { return "c"; }
    // ... можно продолжить, здесь только несколько букв для примера
    if (c == "А") { return "а"; }
    if (c == "Б") { return "б"; }
    return c;
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

### Программа IsPrime - Вычисление простого числа
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
    let i     = 3;

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