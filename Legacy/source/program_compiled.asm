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
	
	; (2 + 4) * 3
	push qword 2
	push qword 4
	push qword 3
	pop rcx
	pop rbx
	imul rbx, rcx
	push qword rbx
	pop rcx
	pop rbx
	add rbx, rcx
	push qword rbx
	pop rbx
	
	sub rsp, 4
	mov qword [rbp-4], rbx
	
	mov rsp, rbp
	ret
	;
	; struct token
	;
	; field: ClassType: int address

ptr__set_value:
	mov rbp, rsp
	mov rbx, [rbp+8]
	mov rcx, [rbp+4]
	mov qword [rbx], rcx
	mov qword [rbx+1], 0x02
	
	mov rsp, rbp
	ret
	;
	; struct token
	;
	