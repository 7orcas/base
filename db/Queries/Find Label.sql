Select l.langCode, l.code, k.code as keyCode 
from base.langLabel l
left join base.langKey k on k.id = l.langKeyId

where l.code like '%Org%' or k.code like '%Org%'