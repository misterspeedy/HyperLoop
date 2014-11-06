module CrossFader

let inline private (~~) x = double x

let CrossFade (count : int) (buffer1 : byte[]) (buffer2 : byte[]) =
   // TODO check equal lengths
   let region1 = buffer1.[buffer2.Length-count..]
   let region2 = buffer2.[..count-1]
   let both = Array.zip region1 region2
   both 
   |> Array.mapi (fun i (a, b) ->
         let m1 = ~~i / ~~count
         let m2 = 1. - m1
         ~~a * m2 + ~~b * m1)
