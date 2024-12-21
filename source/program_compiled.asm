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
	mov qword [rbp-8], 753664
	
	; function call: my_ptr.print_value
	push rbp
	push qword [rbp-4] ; arg 0, 'my_number'
	call ptr__print_value
	add rsp, 4
	pop rbp
	
	sub rsp, 4
	
	; function call: my_ptr.get_value
	push rbp
	push qword [rbp-8] ; arg 0, 'my_ptr'
	call ptr__get_value
	add rsp, 4
	pop rbp
	
	mov qword rbx, [rbp-12]
	mov qword [rbp-4], rbx
	
	; function call: my_ptr.print_value
	push rbp
	push qword [rbp-4] ; arg 0, 'my_number'
	call ptr__print_value
	add rsp, 4
	pop rbp
	
	
	mov rsp, rbp
	ret
	;
	; struct token
	;

ptr__get_value:
	mov rbp, rsp
	sub rsp, 4
	mov qword [rbp-4], 72
	
	mov qword rbx, [rbp-4]
	mov qword [rsp+16], rbx
	mov rsp, rbp
	ret

ptr__print_value:
	mov rbp, rsp
	
	; IntToString number, buffer
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
	