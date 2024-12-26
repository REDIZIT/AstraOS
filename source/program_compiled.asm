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
	mov qword [rbp-8], 0
	sub rsp, 4
	mov qword [rbp-12], 0
.while_check_1:
	
	; y < 7
	push qword [rbp-12]
	push qword 7
	pop rcx
	pop rbx
	cmp rbx, rcx
	setl dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rbx
	
	cmp rbx, 0
	jle .while_exit_1
	sub rsp, 4
	mov qword [rbp-16], 0
.while_check_2:
	
	; x < 3
	push qword [rbp-16]
	push qword 3
	pop rcx
	pop rbx
	cmp rbx, rcx
	setl dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rbx
	
	cmp rbx, 0
	jle .while_exit_2
	
	; 753664 + x * 2 + y * 2 * 80
	push qword 753664
	push qword [rbp-16]
	push qword 2
	pop rcx
	pop rbx
	imul rbx, rcx
	push qword rbx
	pop rcx
	pop rbx
	add rbx, rcx
	push qword rbx
	push qword [rbp-12]
	push qword 2
	pop rcx
	pop rbx
	imul rbx, rcx
	push qword rbx
	push qword 80
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
	mov qword [rbp-8], rbx
	mov qword rbx, [rbp-8]
	mov qword [rbp-8], rbx
	
	; function call: my_ptr.set_value
	push rbp
	push qword [rbp-8] ; arg 0, 'my_ptr'
	push qword 48 ; arg 1, '48'
	call ptr__set_value
	add rsp, 8
	pop rbp
	
	
	inc qword [rbp-16]
	
	jmp .while_check_2
.while_exit_2:
	
	
	inc qword [rbp-12]
	
	jmp .while_check_1
.while_exit_1:
	
	
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
	