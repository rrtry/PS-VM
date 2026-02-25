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
    if (ch == "А") { return "а"; }
    if (ch == "Б") { return "б"; }
    if (ch == "В") { return "в"; }
    if (ch == "Г") { return "г"; }
    if (ch == "Д") { return "д"; }
    if (ch == "Е") { return "е"; }
    if (ch == "Ё") { return "ё"; }
    if (ch == "Ж") { return "ж"; }
    if (ch == "З") { return "з"; }
    if (ch == "И") { return "и"; }
    if (ch == "Й") { return "й"; }
    if (ch == "К") { return "к"; }
    if (ch == "Л") { return "л"; }
    if (ch == "М") { return "м"; }
    if (ch == "Н") { return "н"; }
    if (ch == "О") { return "о"; }
    if (ch == "П") { return "п"; }
    if (ch == "Р") { return "р"; }
    if (ch == "С") { return "с"; }
    if (ch == "Т") { return "т"; }
    if (ch == "У") { return "у"; }
    if (ch == "Ф") { return "ф"; }
    if (ch == "Х") { return "х"; }
    if (ch == "Ц") { return "ц"; }
    if (ch == "Ч") { return "ч"; }
    if (ch == "Ш") { return "ш"; }
    if (ch == "Щ") { return "щ"; }
    if (ch == "Ъ") { return "ъ"; }
    if (ch == "Ы") { return "ы"; }
    if (ch == "Ь") { return "ь"; }
    if (ch == "Э") { return "э"; }
    if (ch == "Ю") { return "ю"; }
    if (ch == "Я") { return "я"; }
    
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