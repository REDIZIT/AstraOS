section .data
width:          dq 80
width_doubled:  dq 160

section .text
global program_с_main

program_с_main:
	mov rcx, 2
	mov rdx, 1
	call set_char

	mov rcx, 2
	mov rdx, 2
	call set_char

	mov rcx, 4
	mov rdx, 1
	call set_char

	mov rcx, 4
	mov rdx, 2
	call set_char


	mov rcx, 1
	mov rdx, 4
	call set_char

	mov rcx, 2
	mov rdx, 5
	call set_char

	mov rcx, 3
	mov rdx, 5
	call set_char

	mov rcx, 4
	mov rdx, 5
	call set_char

	mov rcx, 5
	mov rdx, 4
	call set_char

	ret

set_char:
    mov rbx, 0xB8000

    imul rcx, 2
    add rbx, rcx

    imul rdx, [width_doubled]
    add rbx, rdx

	mov byte [rbx], "@"
	mov byte [rbx+1], 0x07

	ret
