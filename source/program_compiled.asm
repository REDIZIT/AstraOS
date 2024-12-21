section .data
	msg db "String in .data section", 0

section .bss
	buffer resb 32

section .text

extern console_writeline
extern int_to_string

global program__main
	;
	; struct token
	;

program__main:
	mov rbp, rsp
	sub rsp, 4
	mov qword [rbp-4], 42
	sub rsp, 4
	mov qword [rbp-8], 32
	sub rsp, 4
	mov qword [rbp-12], 0
	
	push rbp
	push qword [rbp-12]
	push qword [rbp-4]
	push qword [rbp-8]
	call ptr__set_value
	add rsp, 12
	pop rbp
	
	mov rsp, rbp
	ret
	;
	; struct token
	;

ptr__set_value:
	mov rbp, rsp
	
	; IntToString b, buffer
	mov qword rbx, [rbp + 4]
	push rbx
	push buffer
	call int_to_string
	add rsp, 8
	
	push buffer
	call console_writeline
	add rsp, 4
	
	mov rsp, rbp
	ret
	;
	; struct token
	;
	