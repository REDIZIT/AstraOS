section .multiboot
align 8
dd 0x1BADB002          ; Магическое число Multiboot
dd 0x0                 ; Флаги
dd -(0x1BADB002 + 0x0) ; Контрольная сумма