section .data
	msg db "String in .data section", 0

section .text
global program_сompiled_main
extern console_writeline

program_сompiled_main:
	mov rbp, rsp
	sub rsp, 4
	mov qword [rbp-4], 50
	sub rsp, 4
	mov qword [rbp-8], 2
	
	; a > b + 10 and (b < 100 or a > 150)
	push qword [rbp-4]
	push qword [rbp-8]
	push qword 10
	pop rcx
	pop rbx
	add rbx, rcx
	push qword rbx
	pop rcx
	pop rbx
	cmp rbx, rcx
	setg dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	push qword [rbp-8]
	push qword 100
	pop rcx
	pop rbx
	cmp rbx, rcx
	setl dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rcx
	pop rbx
	and rbx, rcx
	push qword rbx
	push qword [rbp-4]
	push qword 150
	pop rcx
	pop rbx
	cmp rbx, rcx
	setg dl
	movzx rdx, dl
	mov rbx, rdx
	push qword rbx
	pop rcx
	pop rbx
	or rbx, rcx
	push qword rbx
	pop rbx
	
	
	cmp rbx, 1
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
	
	; string str = '123'
	mov rbx, rbp
	sub rbx, 4
	mov byte [rbx+0], "1"
	mov byte [rbx+1], "2"
	mov byte [rbx+2], "3"
	mov byte [rbx+3], 0
	sub rsp, 4
	
	lea rbx, [rbp-4]
	push rbx
	call console_writeline
	add rsp, 4
	
	mov rsp, rbp
	ret
	