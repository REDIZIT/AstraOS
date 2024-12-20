section .data
	msg db "String in .data section", 0

section .text
global program_сompiled_main
extern console_writeline

program_сompiled_main:
    mov rbp, rsp
    sub rsp, 4
    mov qword [rbp-8], 1
    cmp qword [rbp-8], 1
    jge .if_true
    jmp .if_end
.if_true:
    call my_func
    jmp .if_end
.if_end:

    mov rsp, rbp
    ret

my_func:
    mov rbp, rsp

    ; string str = 'My first string'
    mov rbx, rbp
    sub rbx, 16
    mov byte [rbx+0], "M"
    mov byte [rbx+1], "y"
    mov byte [rbx+2], " "
    mov byte [rbx+3], "f"
    mov byte [rbx+4], "i"
    mov byte [rbx+5], "r"
    mov byte [rbx+6], "s"
    mov byte [rbx+7], "t"
    mov byte [rbx+8], " "
    mov byte [rbx+9], "s"
    mov byte [rbx+10], "t"
    mov byte [rbx+11], "r"
    mov byte [rbx+12], "i"
    mov byte [rbx+13], "n"
    mov byte [rbx+14], "g"
    mov byte [rbx+15], 0
    sub rsp, 16

    lea rbx, [rbp-16]
    push rbx
    call console_writeline
    add rsp, 4

    ; string str2 = 'My second string'
    mov rbx, rbp
    sub rbx, 33
    mov byte [rbx+0], "M"
    mov byte [rbx+1], "y"
    mov byte [rbx+2], " "
    mov byte [rbx+3], "s"
    mov byte [rbx+4], "e"
    mov byte [rbx+5], "c"
    mov byte [rbx+6], "o"
    mov byte [rbx+7], "n"
    mov byte [rbx+8], "d"
    mov byte [rbx+9], " "
    mov byte [rbx+10], "s"
    mov byte [rbx+11], "t"
    mov byte [rbx+12], "r"
    mov byte [rbx+13], "i"
    mov byte [rbx+14], "n"
    mov byte [rbx+15], "g"
    mov byte [rbx+16], 0
    sub rsp, 17

    lea rbx, [rbp-33]
    push rbx
    call console_writeline
    add rsp, 4
    lea rbx, [rbp-16]
    push rbx
    call console_writeline
    add rsp, 4
    lea rbx, [rbp-33]
    push rbx
    call console_writeline
    add rsp, 4

    mov rsp, rbp
    ret