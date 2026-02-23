# Примеры программ на языке ----

## 1. SumNumbers
```
fn main()
{
    let a = stoi(input());
    let b = stoi(input());
    let sum = a + b;
    printi(sum);
}
```

---

## 2. CircleSquare
```
fn main()
{
    let r = input();
    let area = Pi * r * r;
    printf(area, 2);
}
```

---
## 3. GeometricMean

```
fn main()
{
    let a = stoi(input());
    let b = stoi(input());
    let gmean = (a * b) ** 0.5;
    printf(gmean, 2);
}
```

### Программа Factorial
```
fn main()
{
    printi(factorial(5)); // 120
}

fn factorial(n): int 
{
    let fact = 1;
    for (let i = 1, i <= n, i = i + 1) 
    {
        fact = fact * i;
    }
    return fact;
}
```

### Программа IsPrime
```
fn main() 
{
    let result = is_prime(5);
    printi(result); // 1
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

### Программа QuadraticEquation

```
fn main()
{
    let a = stof(input());
    let b = stof(input());
    let c = stof(input());
    let result = solve(a, b, c);
    printf(result, 2);
}

fn solve(a: float, b: float, c: float): float
{
    if (a == 0) 
    {
        if (b != 0) 
        {
            let root1 = -c / b;
            printf(root1, 2);
            return 1.0;
        }
        return 0.0;
    }
    else 
    {
        let disc = b * b - 4 * a * c;
        if (disc > 0) 
        {
            let sqrt_disc = pow(disc, 0.5);
            let root1     = (-b + sqrt_disc) / (2 * a);
            let root2     = (-b - sqrt_disc) / (2 * a);
            printf(root1, 2);
            printf(root2, 2);
            return 2.0;
        }
        if (disc == 0) 
        {
            let root1 = -b / (2 * a);
            printf(root1, 2);
            return 1.0;
        } 
        return 0.0;
    }
}
```