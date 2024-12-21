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
	mov qword [rbp-4], msg
	
	push rbp
	push qword [rbp-4]
	call ptr__set_value
	add rsp, 4
	pop rbp
	
	mov rsp, rbp
	ret
	;
	; struct token
	;

ptr__set_value:
	mov rbp, rsp
	
	push qword [rbp+4]
	call console_writeline
	add rsp, 4
	
	mov rsp, rbp
	ret
	