module CrossFader

let inline private (~~) x = double x

let CrossFade (count : int) (buffer1 : byte[]) (buffer2 : byte[]) =
   (buffer1.[buffer2.Length-count..],
    buffer2.[..count-1])
   ||> Array.zip
   |> Array.mapi (fun i (a, b) ->
         let m1 = ~~i / ~~count
         let m2 = 1. - m1
         ~~a * m2 + ~~b * m1)
