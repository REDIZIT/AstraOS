section .data
	msg db "String in .data section", 0

section .bss
	buffer resb 32

section .text

extern console_writeline
extern int_to_string

global program_сompiled_main

program_сompiled_main:
	mov rbp, rsp
	sub rsp, 4
	mov qword [rbp-4], 0
	sub rsp, 4
	mov qword [rbp-8], 10
	sub rsp, 4
	mov qword rbx, [rbp-8]
	mov [rbp-12], rbx
.while_check:
	
	; i >= a
	push qword [rbp-12]
	push qword [rbp-4]
	pop rcx
	pop rbx
	cmp rbx, rcx
	setge dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rbx
	
	cmp rbx, 0
	jle .while_exit
	
	; IntToString i, buffer
	mov qword rbx, [rbp-12]
	push rbx
	push buffer
	call int_to_string
	add rsp, 8
	
	push buffer
	call console_writeline
	add rsp, 4
	
	dec qword [rbp-12]
	
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
	