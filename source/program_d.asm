global program_d_main
extern print_string_stack_coord_locals

section .data
	const10:    dd 10

section .bss
	buffer resb 32


section .text
program_d_main:

	mov eax, 12345
	mov edx, 0
	mov rbx, 0
	call int_to_string

	push buffer  ; "12345"
	push 0
	push 0
	call print_string_stack_coord_locals
	add rsp, 12

	mov rsi, buffer
	call reverse_string

	push buffer  ; "54321"
	push 0
	push 1
	call print_string_stack_coord_locals
	add rsp, 12

	ret



; Input
; eax - number
int_to_string:

	; Divide EAX by 10
	; div - Unsigned divide EDX:EAX by r/m32, with result stored in EAX := Quotient, EDX := Remainder

	div dword [const10]

	push rax

	; Convert int (digit) (inside EDX) to ASCII char (inside ECX)
	mov ecx, edx
	add ecx, '0'

	; Move char to buffer
	mov [buffer+rbx], cl
	add rbx, 1

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .int_to_string_exit
	; else
	jmp int_to_string
.int_to_string_exit:
	; then
	mov rsi, buffer
	call reverse_string
	ret


; input
; rsi - pointer to source string
; rdi - pointer to dest string
reverse_string:
	mov rbx, 0  ; len

.get_len:
	mov rcx, [rsi+rbx]  ; string[rbx]
	test rcx, rcx
	je .l1  ; if char is 0?
	; else
	add rbx, 1
	jmp .get_len
.l1:
	; then
	jmp .swap

.swap:
	; rax - 
	; rbx - len => j
	; rcx - i
	; rdx - temp
	; rsi, rdi - pointers

	sub rbx, 1  ; j
	mov rcx, 0  ; i

	; while (i < j)
.while:
	cmp rbx, rcx
	jng .end  ; if (i < j)
	; body

	; temp = str[i]
	mov byte al, [rsi+rcx]

	; str[i] = str[j]
	mov byte dl, [rsi+rbx]
	mov byte [rsi+rcx], dl

	; str[j] = temp
	mov byte [rsi+rbx], al


	add rcx, 1
	sub rbx, 1

	jmp .while
.end:
	; break
	ret