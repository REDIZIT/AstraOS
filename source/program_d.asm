global program_d_main
extern print_string_stack_coord_locals

section .data
	test_str dd "My int = ", 0
	test_value dd "My value", 0
	const10:    dd 10

section .bss
	buffer resb 128
	buffer2 resb 128


section .text
program_d_main:

	;push test_str
	;push 0
	;push 0
	;call print_string_stack_coord_locals
	;add rsp, 12

	;push test_value
	;push 9
	;push 0
	;call print_string_stack_coord_locals
	;add rsp, 12


	;mov eax, 1203
	;mov edx, 0
	;mov rbx, 0xB8000
	;call printNumber



	mov eax, 1203
	mov edx, 0
	mov rbx, 0
	call int_to_string

	push buffer,
	push 0,
	push 0,
	call print_string_stack_coord_locals
	add rsp, 12

	mov rsi, buffer
	mov rdi, buffer2
	call reverse_string

	;call reverse

	push buffer,
	push 0,
	push 1,
	call print_string_stack_coord_locals
	add rsp, 12

	ret



printNumber:

	; Divide EAX by 10
	; div - Unsigned divide EDX:EAX by r/m32, with result stored in EAX := Quotient, EDX := Remainder




	div dword [const10]

	push rax

	; Convert int (digit) (inside EDX) to ASCII char (inside ECX)
	mov ecx, edx
	add ecx, '0'

	; Print char (inside ECX)
	mov byte [rbx], cl
	mov byte [rbx+1], 0x0f
	add rbx, 2

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .l1
	; else
	call printNumber
.l1:
	; then
	ret


; Input
; eax - number
; rsi - pointer to string
int_to_string:

	; Divide EAX by 10
	; div - Unsigned divide EDX:EAX by r/m32, with result stored in EAX := Quotient, EDX := Remainder




	div dword [const10]

	push rax

	; Convert int (digit) (inside EDX) to ASCII char (inside ECX)
	mov ecx, edx
	add ecx, '0'

	;; Print char (inside ECX)
	;mov byte [rbx], cl
	;mov byte [rbx+1], 0x2f
	;add rbx, 2

	; Move char to buffer
	mov [buffer+rbx], cl
	add rbx, 1

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .l1
	; else
	call int_to_string
.l1:
	; then
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
	; 

	sub rbx, 1  ; j
	mov rcx, 0  ; i

	;mov byte [rsi], "H"

	; while (i < j)
.while:
	cmp rbx, rcx
	jng .end  ; if (i < j)
	; body
	;;mov byte [rsi], "B"

	;; temp = str[i]
	;mov dl, [rsi+rcx]

	;; str[i] = str[j]
	;mov rax, [rsi+rbx]  ; str[j]
	;mov [rsi+rcx], rax  ; str[i]

	;; str[j] = temp
	;mov [rsi+rbx], dl

	;mov byte [rsi+rcx], "F"



	;mov rax, [rsi+rcx]
	;mov byte [rsi+rbx], al


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

	;mov rdi, 0xB8000
	;mov byte [rdi], "X"
	;mov byte [rdi+1], 0x1f

.end:
	; break
	;mov rdi, 0xB8000
	;mov byte [rdi], "X"
	;mov byte [rdi+1], 0x2f
	;mov byte [rsi+1], "G"
	ret





reverse:
	;push rbp           ; prologue
	;mov ebp, esp       
	;mov eax, [ebp+4]   ; eax <- points to string
	mov rdx, buffer   ; eax <- points to string
	;mov byte [rax], "F"
	mov rax, rdx
look_for_last:
	mov ch, [rdx]      ; put char from edx in ch
	inc rdx
	test ch, ch        
	jnz look_for_last  ; if char != 0 loop
	sub rdx, 2         ; found last
swap2:                      ; eax = first, edx = last (characters in string)
	cmp rax, rdx
	jg end             ; if eax > edx reverse is done
	mov cl, [rax]      ; put char from eax in cl
	mov ch, [rdx]      ; put char from edx in ch
	mov [rdx], cl      ; put cl in edx
	mov [rax], ch      ; put ch in eax
	inc rax
	dec rdx
	;mov byte [rdx], "F"
	ret
	jmp swap2
end:
	;mov eax, [ebp+4]   ; move char pointer to eax (func return)
	;mov rax, buffer   ; move char pointer to eax (func return)
	;pop rbp            ; epilogue
	ret


;Input
; eax = number to display
printNumber2:
	push rax
	push rdx
	xor rdx,rdx          ;edx:eax = number
	div dword [const10]  ;eax = quotient, edx = remainder
	test rax,rax         ;Is quotient zero?
	je .l1               ; yes, don't display it
	call printNumber     ;Display the quotient
.l1:
	lea rax,[rdx+'0']
	call printCharacter  ;Display the remainder
	pop rdx
	pop rax
	ret


printCharacter:
	mov rdi, 0xB8000
	mov byte [rdi], "F"
	mov byte [rdi+1], 0x1f
	ret