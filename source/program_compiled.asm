section .data
	msg_old db "Old way used", 0

section .text
global program_сompiled_main
extern console_writeline


program_сompiled_main:
	call my_func
    ret

my_func:
    mov rbp, rsp

    ; string str = 'My string'
    mov rbx, rbp
    sub rbx, 10
    mov byte [rbx+0], "M"
    mov byte [rbx+1], "y"
    mov byte [rbx+2], " "
    mov byte [rbx+3], "s"
    mov byte [rbx+4], "t"
    mov byte [rbx+5], "r"
    mov byte [rbx+6], "i"
    mov byte [rbx+7], "n"
    mov byte [rbx+8], "g"
    mov byte [rbx+9], 0
    sub rsp, 10

    lea rbx, [rbp-10]
    push rbx
    call console_writeline
    add rsp, 4

    ; string str2 = 'My another string'
    mov rbx, rbp
    sub rbx, 28
    mov byte [rbx+0], "M"
    mov byte [rbx+1], "y"
    mov byte [rbx+2], " "
    mov byte [rbx+3], "a"
    mov byte [rbx+4], "n"
    mov byte [rbx+5], "o"
    mov byte [rbx+6], "t"
    mov byte [rbx+7], "h"
    mov byte [rbx+8], "e"
    mov byte [rbx+9], "r"
    mov byte [rbx+10], " "
    mov byte [rbx+11], "s"
    mov byte [rbx+12], "t"
    mov byte [rbx+13], "r"
    mov byte [rbx+14], "i"
    mov byte [rbx+15], "n"
    mov byte [rbx+16], "g"
    mov byte [rbx+17], 0
    sub rsp, 18

    lea rbx, [rbp-28]
    push rbx
    call console_writeline
    add rsp, 4
    lea rbx, [rbp-10]
    push rbx
    call console_writeline
    add rsp, 4
    lea rbx, [rbp-28]
    push rbx
    call console_writeline
    add rsp, 4

    mov rsp, rbp
    ret