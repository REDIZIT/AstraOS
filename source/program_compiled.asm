section .data
	msg db "String in .data section", 0

section .text
global program_сompiled_main
extern console_writeline

program_сompiled_main:
	mov rbp, rsp
	sub rsp, 4
	mov qword [rbp-4], 3
.while_check:
	
	; i > 0
	push qword [rbp-4]
	push qword 0
	pop rcx
	pop rbx
	cmp rbx, rcx
	setg dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rbx
	
	cmp rbx, 0
	jle .while_exit
	push rbp
	call my_func1
	pop rbp
	
	; i - 1
	push qword [rbp-4]
	push qword 1
	pop rcx
	pop rbx
	sub rbx, rcx
	push qword rbx
	pop rbx
	mov [rbp-4], rbx
	; MathExpressions.Generate
	
	jmp .while_check
.while_exit:
	
	push rbp
	call my_func2
	pop rbp
	
	mov rsp, rbp
	ret

my_func1:
	mov rbp, rsp
	
	; string str = 'body'
	mov rbx, rbp
	sub rbx, 5
	mov byte [rbx+0], "b"
	mov byte [rbx+1], "o"
	mov byte [rbx+2], "d"
	mov byte [rbx+3], "y"
	mov byte [rbx+4], 0
	sub rsp, 5
	
	lea rbx, [rbp-5]
	push rbx
	call console_writeline
	add rsp, 4
	
	mov rsp, rbp
	ret

my_func2:
	mov rbp, rsp
	
	; string str = '!!! exit !!!'
	mov rbx, rbp
	sub rbx, 13
	mov byte [rbx+0], "!"
	mov byte [rbx+1], "!"
	mov byte [rbx+2], "!"
	mov byte [rbx+3], " "
	mov byte [rbx+4], "e"
	mov byte [rbx+5], "x"
	mov byte [rbx+6], "i"
	mov byte [rbx+7], "t"
	mov byte [rbx+8], " "
	mov byte [rbx+9], "!"
	mov byte [rbx+10], "!"
	mov byte [rbx+11], "!"
	mov byte [rbx+12], 0
	sub rsp, 13
	
	lea rbx, [rbp-13]
	push rbx
	call console_writeline
	add rsp, 4
	
	mov rsp, rbp
	ret
	