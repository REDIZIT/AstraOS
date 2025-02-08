Compiler.Main();
return;

void PrintNumber(int number)
{
    int a = number / 10;
    Console.WriteLine(a);

    if (a > 0)
    {

    }


}

PrintNumber(42);


char[] str = "Hello world!".ToCharArray();
char temp;

int j, i;
i = 0;
j = str.Length - 1;
while (i < j) {
    temp = str[i];
    str[i] = str[j];
    str[j] = temp;
    i++;
    j--;
}

Console.WriteLine(new string(str));