name=alfresco-group-import
mkdir /tmp/$name
version=$(curl https://api.github.com/repos/wk-j/$name/releases/latest | grep -Eo "\"tag_name\":\s*\"(.*)\"" | cut -d'"' -f4)
echo "Installing $version..."
curl -L https://github.com/wk-j/$name/releases/download/$version/$name.$version.zip > /tmp/$name/$name.zip
unzip -o /tmp/$name/$name.zip -d /usr/local/lib/$name
chmod +x /usr/local/lib/$name/$name.sh
cd /usr/local/bin
ln -sfn /usr/local/lib/$name/$name.sh $name 
rm -rf /tmp/$name